using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Scanner
{
    enum TokenType
    {
        LeftParenthesis,
        RightParenthesis,
        LeftBrace,
        RightBrace,
        Comma,
        Dot,
        Minus,
        Plus,
        Semicolon,
        Colon,
        Slash,
        Star,

        Bang,
        BangEqual,
        Equal,
        EqualEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,

        Identifier,
        String,
        Number,

        Print,

        And,
        Class,
        Else,
        False,
        Func,
        For,
        If,
        Nil,
        Or,
        Return,
        Super,
        This,
        True,
        Var,
        While,

        EOF
    }
}
