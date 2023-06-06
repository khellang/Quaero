using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Quaero;

internal static class FilterTokenizer
{
    private static TextParser<Unit> FilterStringToken { get; } =
        from open in Character.EqualTo('\'')
        from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
            .Or(Character.Except('\'').Value(Unit.Value))
            .IgnoreMany()
        from close in Character.EqualTo('\'')
        select Unit.Value;

    private static TextParser<Unit> FilterNumberToken { get; } =
        from sign in Character.EqualTo('-').OptionalOrDefault()
        from first in Character.Digit
        from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
        select Unit.Value;

    public static Tokenizer<FilterToken> Instance { get; } =
        new TokenizerBuilder<FilterToken>()
            .Ignore(Span.WhiteSpace)
            .Match(Span.EqualToIgnoreCase("eq"), FilterToken.Equal, requireDelimiters: true)
            .Match(Span.EqualToIgnoreCase("ne"), FilterToken.NotEqual, requireDelimiters: true)
            .Match(Span.EqualToIgnoreCase("lt"), FilterToken.LessThan, requireDelimiters: true)
            .Match(Span.EqualToIgnoreCase("le"), FilterToken.LessThanOrEqual, requireDelimiters: true)
            .Match(Span.EqualToIgnoreCase("gt"), FilterToken.GreaterThan, requireDelimiters: true)
            .Match(Span.EqualToIgnoreCase("ge"), FilterToken.GreaterThanOrEqual, requireDelimiters: true)
            .Match(Span.EqualToIgnoreCase("startsWith"), FilterToken.StartsWith, requireDelimiters: true)
            .Match(Span.EqualToIgnoreCase("endsWith"), FilterToken.EndsWith, requireDelimiters: true)
            .Match(Character.EqualTo('('), FilterToken.LParen)
            .Match(Character.EqualTo(')'), FilterToken.RParen)
            .Match(FilterStringToken, FilterToken.String)
            .Match(FilterNumberToken, FilterToken.Number, requireDelimiters: true)
            .Match(Identifier.CStyle, FilterToken.Identifier, requireDelimiters: true)
            .Build();
}
