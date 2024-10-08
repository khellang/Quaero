using System.Numerics;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Quaero;

internal static class FilterParser
{
    private static TokenListParser<FilterToken, object> String { get; } =
        Token.EqualTo(FilterToken.String)
            .Apply(QuotedString.CStyle)
            .Select(g => (object)g);

    private static TokenListParser<FilterToken, object> Guid { get; } =
        Token.EqualTo(FilterToken.Guid)
            .Apply(FilterTextParsers.Guid)
            .Select(s => (object)s);

    private static TokenListParser<FilterToken, object> Integer { get; } =
        Token.EqualTo(FilterToken.Integer)
            .Apply(Numerics.Integer)
            .Select(n => ParseInteger(n.ToStringValue()));

    private static object ParseInteger(string value)
    {
        if (int.TryParse(value, out var intValue))
        {
            return intValue;
        }

        if (long.TryParse(value, out var longValue))
        {
            return longValue;
        }

        if (BigInteger.TryParse(value, out var bigInteger))
        {
            return bigInteger;
        }

        throw new ParseException($"Value '{value}' is not a valid integer.");
    }

    private static TokenListParser<FilterToken, object> Decimal { get; } =
        Token.EqualTo(FilterToken.Decimal)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (object)n);

    private static TokenListParser<FilterToken, object> True { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "true").Value((object)true);

    private static TokenListParser<FilterToken, object> False { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "false").Value((object)false);

    private static TokenListParser<FilterToken, object?> Null { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "null").Value((object?)null);

    private static TokenListParser<FilterToken, LogicalOperator> NotOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "not").Value(LogicalOperator.Not);

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
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "sw").Value(PropertyOperator.StartsWith);

    private static TokenListParser<FilterToken, PropertyOperator> EndsWithOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "ew").Value(PropertyOperator.EndsWith);

    private static TokenListParser<FilterToken, PropertyOperator> ContainsOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "co").Value(PropertyOperator.Contains);

    private static TokenListParser<FilterToken, PropertyOperator> InOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "in").Value(PropertyOperator.In);

    private static TokenListParser<FilterToken, PropertyOperator> PresenceOperator { get; } =
        Token.EqualToValueIgnoreCase(FilterToken.Identifier, "pr").Value(PropertyOperator.Presence);

    private static TokenListParser<FilterToken, PropertyOperator> PropertyOperators { get; } =
        EqualOperator
            .Or(NotEqualOperator)
            .Or(LessThanOperator)
            .Or(LessThanOrEqualOperator)
            .Or(GreaterThanOperator)
            .Or(GreaterThanOrEqualOperator)
            .Or(StartsWithOperator)
            .Or(EndsWithOperator)
            .Or(ContainsOperator)
            .Or(InOperator)
            .Named("property operator");

    private static TokenListParser<FilterToken, LogicalOperator> BinaryOperators { get; } =
        AndOperator.Or(OrOperator).Named("binary operator");

    private static TokenListParser<FilterToken, object> List { get; } =
        Parse.Ref(() => Value!)
            .AtLeastOnceDelimitedBy(Token.EqualTo(FilterToken.Comma))
            .Between(Token.EqualTo(FilterToken.OpenBracket), Token.EqualTo(FilterToken.CloseBracket))
            .Select(x => (object)x);

    private static TokenListParser<FilterToken, object?> Value { get; } =
        String.AsNullable()
            .Or(Guid.AsNullable())
            .Or(Integer.AsNullable())
            .Or(Decimal.AsNullable())
            .Or(True.AsNullable())
            .Or(False.AsNullable())
            .Or(List.AsNullable())
            .Or(Null)
            .Named("value");

    private static TokenListParser<FilterToken, Filter> PredicateWithoutArgument { get; } =
        from identifier in Token.EqualTo(FilterToken.Identifier)
        from @operator in PresenceOperator
        select Filter.Present(identifier.ToStringValue());

    private static TokenListParser<FilterToken, Filter>  PredicateWithArgument { get; } =
        from identifier in Token.EqualTo(FilterToken.Identifier)
        from @operator in PropertyOperators
        from value in Value
        select GetFilter(identifier, @operator, value);

    private static TokenListParser<FilterToken, Filter> Predicate { get; } =
        PredicateWithoutArgument.Try().Or(PredicateWithArgument);

    private static TokenListParser<FilterToken, Filter> Group { get; } =
        Parse.Ref(() => Expression!)
            .Between(Token.EqualTo(FilterToken.OpenParen), Token.EqualTo(FilterToken.CloseParen));

    private static TokenListParser<FilterToken, Filter> Factor { get; } =
        Group.Or(Predicate);

    private static TokenListParser<FilterToken, Filter> Negated { get; } =
        from op in NotOperator
        from val in Group
        select Filter.Not(val);

    private static TokenListParser<FilterToken, Filter> Operand { get; } =
        Negated.Or(Factor);

    private static TokenListParser<FilterToken, Filter> Expression { get; } = Parse.Chain(BinaryOperators, Operand, MakeLogicalOperator);

    public static TokenListParser<FilterToken, Filter> Instance { get; } = Expression.AtEnd();

    private static Filter MakeLogicalOperator(LogicalOperator @operator, Filter left, Filter right) => @operator switch
    {
        LogicalOperator.And => left.And(right),
        LogicalOperator.Or => left.Or(right),
        _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Invalid logical operator."),
    };

    private static Filter GetFilter(Token<FilterToken> nameToken, PropertyOperator @operator, object? value)
    {
        var name = nameToken.ToStringValue();

        return @operator switch
        {
            PropertyOperator.Equal => CreateFilter(typeof(EqualFilter<>), name, value),
            PropertyOperator.NotEqual => CreateFilter(typeof(NotEqualFilter<>), name, value),
            PropertyOperator.LessThan => CreateFilter(typeof(LessThanFilter<>), name, value),
            PropertyOperator.LessThanOrEqual => CreateFilter(typeof(LessThanOrEqualFilter<>), name, value),
            PropertyOperator.GreaterThan => CreateFilter(typeof(GreaterThanFilter<>), name, value),
            PropertyOperator.GreaterThanOrEqual => CreateFilter(typeof(GreaterThanOrEqualFilter<>), name, value),
            PropertyOperator.StartsWith => Filter.StartsWith(name, GetString(value)),
            PropertyOperator.EndsWith => Filter.EndsWith(name, GetString(value)),
            PropertyOperator.Contains => Filter.Contains(name, GetString(value)),
            PropertyOperator.In => Filter.In(name, GetList(value)),
            _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Invalid property operator."),
        };
    }

    private static string? GetString(object? value) => value switch
    {
        null => null,
        string str => str,
        Unit => throw new ParseException("Operator requires a string argument."),
        _ => throw new ParseException($"Value of type '{FormatType(value)}' is not supported. Expected a string.")
    };

    private static object[] GetList(object? value) => value switch
    {
        object[] list => list,
        Unit => throw new ParseException("Operator requires a list argument."),
        _ => throw new ParseException($"Value of type '{FormatType(value)}' is not supported. Expected a list.")
    };

    private static string FormatType(object? value) => value?.GetType().ToString() ?? "null";

    private static Filter CreateFilter(Type filterType, string name, object? value)
    {
        if (value is Unit)
        {
            throw new ParseException("Operator requires an argument.");
        }

        var type = filterType.MakeGenericType(value?.GetType() ?? typeof(object));
        return (Filter)Activator.CreateInstance(type, name, value)!;
    }

    private enum LogicalOperator
    {
        And,

        Or,

        Not,
    }

    private enum PropertyOperator
    {
        Equal,

        NotEqual,

        LessThan,

        LessThanOrEqual,

        GreaterThan,

        GreaterThanOrEqual,

        StartsWith,

        EndsWith,

        Contains,

        Presence,

        In,
    }
}
