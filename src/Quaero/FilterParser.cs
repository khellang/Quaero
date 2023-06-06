using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Quaero;

internal static class FilterParser
{
    private static TokenListParser<FilterToken, object> String { get; } =
        Token.EqualTo(FilterToken.String)
            .Apply(FilterTextParsers.String)
            .Select(g => (object)g);

    private static TokenListParser<FilterToken, object> Guid { get; } =
        Token.EqualTo(FilterToken.Guid)
            .Apply(FilterTextParsers.Guid)
            .Select(s => (object)s);

    private static TokenListParser<FilterToken, object> Number { get; } =
        Token.EqualTo(FilterToken.Number)
            .Apply(FilterTextParsers.Number)
            .Select(n => (object)n);

    private static TokenListParser<FilterToken, object> True { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "true").Value((object)true);

    private static TokenListParser<FilterToken, object> False { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "false").Value((object)false);

    private static TokenListParser<FilterToken, object?> Null { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "null").Value((object?)null);

    private static TokenListParser<FilterToken, LogicalOperator> AndOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "and").Value(LogicalOperator.And);

    private static TokenListParser<FilterToken, LogicalOperator> OrOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "or").Value(LogicalOperator.Or);

    private static TokenListParser<FilterToken, PropertyOperator> EqualOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "eq").Value(PropertyOperator.Equal);

    private static TokenListParser<FilterToken, PropertyOperator> NotEqualOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "ne").Value(PropertyOperator.NotEqual);

    private static TokenListParser<FilterToken, PropertyOperator> LessThanOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "lt").Value(PropertyOperator.LessThan);

    private static TokenListParser<FilterToken, PropertyOperator> LessThanOrEqualOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "le").Value(PropertyOperator.LessThanOrEqual);

    private static TokenListParser<FilterToken, PropertyOperator> GreaterThanOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "gt").Value(PropertyOperator.GreaterThan);

    private static TokenListParser<FilterToken, PropertyOperator> GreaterThanOrEqualOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "ge").Value(PropertyOperator.GreaterThanOrEqual);

    private static TokenListParser<FilterToken, PropertyOperator> StartsWithOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "startsWith").Value(PropertyOperator.StartsWith);

    private static TokenListParser<FilterToken, PropertyOperator> EndsWithOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "endsWith").Value(PropertyOperator.EndsWith);

    private static TokenListParser<FilterToken, PropertyOperator> PropertyOperators { get; } =
        EqualOperator
            .Or(NotEqualOperator)
            .Or(LessThanOperator)
            .Or(LessThanOrEqualOperator)
            .Or(GreaterThanOperator)
            .Or(GreaterThanOrEqualOperator)
            .Or(StartsWithOperator)
            .Or(EndsWithOperator)
            .Named("property operator");

    private static TokenListParser<FilterToken, LogicalOperator> BinaryOperators { get; } =
        AndOperator.Or(OrOperator).Named("binary operator");

    private static TokenListParser<FilterToken, object?> Value { get; } =
        String.AsNullable()
            .Or(Guid.AsNullable())
            .Or(Number.AsNullable())
            .Or(True.AsNullable())
            .Or(False.AsNullable())
            .Or(Null)
            .Named("value");

    private static TokenListParser<FilterToken, Filter> Predicate { get; } =
        from identifier in Token.EqualTo(FilterToken.Identifier)
        from @operator in PropertyOperators
        from value in Value
        select GetFilter(identifier, @operator, value);

    private static readonly TokenListParser<FilterToken, Filter> Group =
        (from lparen in Token.EqualTo(FilterToken.LParen)
         from expr in Parse.Ref(() => Expression!)
         from rparen in Token.EqualTo(FilterToken.RParen)
         select expr)
        .Or(Predicate);

    private static TokenListParser<FilterToken, Filter> Operand { get; } =
        (from @operator in Token.EqualToValueIgnoreCase(FilterToken.Identifier, "not")
         from predicate in Group
         select Filter.Not(predicate))
        .Or(Group)
        .Named("expression");

    private static TokenListParser<FilterToken, Filter> Expression { get; } = Parse.Chain(BinaryOperators, Operand, MakeLogicalOperator);

    public static TokenListParser<FilterToken, Filter> Instance { get; } = Expression.AtEnd();

    private static Filter MakeLogicalOperator(LogicalOperator @operator, Filter left, Filter right) => @operator switch
    {
        LogicalOperator.And => Filter.And(left, right),
        LogicalOperator.Or => Filter.Or(left, right),
        _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Invalid logical operator."),
    };

    private static Filter GetFilter(Token<FilterToken> nameToken, PropertyOperator @operator, object? value)
    {
        var name = nameToken.ToStringValue();

        return @operator switch
        {
            PropertyOperator.Equal => GetEqualFilter(name, value),
            PropertyOperator.NotEqual => GetNotEqualFilter(name, value),
            PropertyOperator.LessThan => Filter.LessThan(name, GetComparable(value)),
            PropertyOperator.LessThanOrEqual => Filter.LessThanOrEqual(name, GetComparable(value)),
            PropertyOperator.GreaterThan => Filter.GreaterThan(name, GetComparable(value)),
            PropertyOperator.GreaterThanOrEqual => Filter.GreaterThanOrEqual(name, GetComparable(value)),
            PropertyOperator.StartsWith => Filter.StartsWith(name, GetString(value)),
            PropertyOperator.EndsWith => Filter.EndsWith(name, GetString(value)),
            _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Invalid property operator."),
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
