using Superpower.Display;

namespace Quaero;

internal enum FilterToken
{
    Number,

    String,

    Guid,

    Identifier,

    [Token(Example = "(")]
    LParen,

    [Token(Example = ")")]
    RParen,
}
