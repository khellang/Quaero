using Superpower.Display;

namespace Quaero;

internal enum FilterToken
{
    Integer,

    Decimal,

    String,

    Guid,

    Identifier,

    [Token(Example = "(")]
    OpenParen,

    [Token(Example = ")")]
    CloseParen,

    [Token(Example = "[")]
    OpenBracket,

    [Token(Example = "]")]
    CloseBracket,

    [Token(Example = ",")]
    Comma,
}
