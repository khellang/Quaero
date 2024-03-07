using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Quaero;

internal static class FilterTokenizer
{
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

    public static Tokenizer<FilterToken> Instance { get; } =
        new TokenizerBuilder<FilterToken>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.EqualTo('('), FilterToken.OpenParen)
            .Match(Character.EqualTo(')'), FilterToken.CloseParen)
            .Match(Character.EqualTo(','), FilterToken.Comma)
            .Match(Guid, FilterToken.Guid, requireDelimiters: true)
            .Match(QuotedString.CStyle, FilterToken.String, requireDelimiters: true)
            .Match(Numerics.DecimalDouble, FilterToken.Decimal, requireDelimiters: true)
            .Match(Numerics.IntegerInt64, FilterToken.Integer, requireDelimiters: true)
            .Match(Identifier.CStyle, FilterToken.Identifier, requireDelimiters: true)
            .Build();
}
