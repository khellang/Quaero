using Superpower.Display;

namespace Quaero;

internal enum FilterToken
{
    Number,

    String,

    Identifier,

    [Token(Example = "(")]
    LParen,

    [Token(Example = ")")]
    RParen,
}
