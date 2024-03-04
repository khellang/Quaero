using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Quaero;

internal static class FilterTokenizer
{
    private static TextParser<Unit> String { get; } =
        from open in Character.EqualTo('\"')
        from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
            .Or(Character.Except('\"').Value(Unit.Value))
            .IgnoreMany()
        from close in Character.EqualTo('\"')
        select Unit.Value;

    private static TextParser<Unit> Guid { get; } =
        from open in Character.EqualTo('\"')
        from content in Character.HexDigit.Repeat(8)
            .IgnoreThen(Character.EqualTo('-'))
            .IgnoreThen(Character.HexDigit.Repeat(4))
            .IgnoreThen(Character.EqualTo('-'))
            .IgnoreThen(Character.HexDigit.Repeat(4))
            .IgnoreThen(Character.EqualTo('-'))
            .IgnoreThen(Character.HexDigit.Repeat(4))
            .IgnoreThen(Character.EqualTo('-'))
            .IgnoreThen(Character.HexDigit.Repeat(12))
        from close in Character.EqualTo('\"')
        select Unit.Value;

    private static TextParser<Unit> Number { get; } =
        from sign in Character.EqualTo('-').OptionalOrDefault()
        from first in Character.Digit
        from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
        select Unit.Value;

    public static Tokenizer<FilterToken> Instance { get; } =
        new TokenizerBuilder<FilterToken>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.EqualTo('('), FilterToken.OpenParen)
            .Match(Character.EqualTo(')'), FilterToken.CloseParen)
            .Match(Character.EqualTo(','), FilterToken.Comma)
            .Match(Guid, FilterToken.Guid, requireDelimiters: true)
            .Match(String, FilterToken.String, requireDelimiters: true)
            .Match(Number, FilterToken.Number, requireDelimiters: true)
            .Match(Identifier.CStyle, FilterToken.Identifier, requireDelimiters: true)
            .Build();
}
