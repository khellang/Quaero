using Superpower.Display;

namespace Quaero;

public enum FilterToken
{
    Number,
    
    String,
    
    Identifier,
    
    [Token(Example = "(")]
    LParen,
    
    [Token(Example = ")")]
    RParen,
}