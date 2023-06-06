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

    [Token(Category = "operator", Example = "eq")]
    Equal,

    [Token(Category = "operator", Example = "ne")]
    NotEqual,

    [Token(Category = "operator", Example = "lt")]
    LessThan,

    [Token(Category = "operator", Example = "le")]
    LessThanOrEqual,

    [Token(Category = "operator", Example = "gt")]
    GreaterThan,

    [Token(Category = "operator", Example = "ge")]
    GreaterThanOrEqual,

    [Token(Category = "operator", Example = "startsWith")]
    StartsWith,

    [Token(Category = "operator", Example = "endsWith")]
    EndsWith,
}
