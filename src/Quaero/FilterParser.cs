using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Quaero;

public static class FilterParser
{
    private static TokenListParser<FilterToken, object> String { get; } =
        Token.EqualTo(FilterToken.String)
            .Apply(FilterTextParsers.String)
            .Select(s => (object)s);
    
    private static TokenListParser<FilterToken, object> Number { get; } =
        Token.EqualTo(FilterToken.Number)
            .Apply(FilterTextParsers.Number)
            .Select(n => (object)n);

    private static TokenListParser<FilterToken, object> True { get; } =
        Token.EqualToValue(FilterToken.Identifier, "true").Value((object)true);
    
    private static TokenListParser<FilterToken, object> False { get; } =
        Token.EqualToValue(FilterToken.Identifier, "false").Value((object)false);
    
    private static TokenListParser<FilterToken, object?> Null { get; } =
        Token.EqualToValue(FilterToken.Identifier, "null").Value((object?)null);

    private static TokenListParser<FilterToken, BinaryOperator> AndOperator { get; } =
        Token.EqualToValue(FilterToken.Identifier, "and").Value(BinaryOperator.And);

    private static TokenListParser<FilterToken, BinaryOperator> OrOperator { get; } =
        Token.EqualToValue(FilterToken.Identifier, "or").Value(BinaryOperator.Or);

    private static TokenListParser<FilterToken, BinaryOperator> BinaryOperators { get; } =
        AndOperator.Or(OrOperator);

    private static TokenListParser<FilterToken, object?> Value { get; } =
        String.AsNullable()
            .Or(Number.AsNullable())
            .Or(True.AsNullable())
            .Or(False.AsNullable())
            .Or(Null)
            .Named("Filter value");
    
    private static TokenListParser<FilterToken, Filter> Predicate { get; } =
        from identifier in Token.EqualTo(FilterToken.Identifier)
        from @operator in Token.EqualTo(FilterToken.Identifier)
        from operand in Value
        select GetFilter(identifier, @operator, operand);

    private static readonly TokenListParser<FilterToken, Filter> Group =
        (from lparen in Token.EqualTo(FilterToken.LParen)
            from expr in Parse.Ref(() => Expression!)
            from rparen in Token.EqualTo(FilterToken.RParen)
            select expr)
        .Or(Predicate);

    private static TokenListParser<FilterToken, Filter> Operand { get; } =
        (from @operator in Token.EqualToValue(FilterToken.Identifier, "not")
            from predicate in Group
            select Filter.Not(predicate))
        .Or(Group)
        .Named("Expression");

    private static TokenListParser<FilterToken, Filter> Expression { get; } = Parse.Chain(BinaryOperators, Operand, MakeBinary);

    public static TokenListParser<FilterToken, Filter> Instance { get; } = Expression.AtEnd();

    private static Filter MakeBinary(BinaryOperator @operator, Filter left, Filter right) => @operator switch
    {
        BinaryOperator.And => Filter.And(left, right),
        BinaryOperator.Or => Filter.Or(left, right),
        _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Invalid operator."),
    };

    private static Filter GetFilter(Token<FilterToken> nameToken, Token<FilterToken> operatorToken, object? value)
    {
        var @operator = operatorToken.ToStringValue();
        var name = nameToken.ToStringValue();
        
        return @operator switch
        {
            "eq" => GetEqualFilter(name, value),
            "ne" => GetNotEqualFilter(name, value),
            "lt" => new LessThanFilter(name, GetComparable(value)),
            "le" => new LessThanOrEqualFilter(name, GetComparable(value)),
            "gt" => new GreaterThanFilter(name, GetComparable(value)),
            "ge" => new GreaterThanOrEqualFilter(name, GetComparable(value)),
            "startsWith" => new StartsWithFilter(name, GetString(value)),
            "endsWith" => new EndsWithFilter(name, GetString(value)),
            _ => throw new ParseException($"Unknown operator: {@operator}", operatorToken.Position),
        };
    }

    private static IComparable GetComparable(object? value) => value switch
    {
        IComparable comparable => comparable,
        _ => throw new ParseException($"Value {value} is not supported.")
    };

    private static string? GetString(object? value) => value switch
    {
        null => null,
        string str => str,
        _ => throw new ParseException($"Value {value} is not supported.")
    };

    private static Filter GetEqualFilter(string name, object? value) => 
        CreateFilter(typeof(EqualFilter<>), name, value);

    private static Filter GetNotEqualFilter(string name, object? value) => 
        CreateFilter(typeof(NotEqualFilter<>), name, value);

    private static Filter CreateFilter(Type filterType, string name, object? value)
    {
        var type = filterType.MakeGenericType(value?.GetType() ?? typeof(object));
        return (Filter)Activator.CreateInstance(type, name, value);
    }
}