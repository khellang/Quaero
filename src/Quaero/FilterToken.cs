using Superpower.Display;

namespace Quaero;

internal enum FilterToken
{
    Number,

    String,

    Guid,

    Identifier,

    [Token(Example = "(")]
    OpenParen,

    [Token(Example = ")")]
    CloseParen,

    [Token(Example = ",")]
    Comma,
}
