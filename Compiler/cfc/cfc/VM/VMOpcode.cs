﻿namespace CodingFoxLang.Compiler
{
    internal enum VMOpcode
    {
        Return,
        Constant,
        Negate,
        Add,
        Subtract,
        Multiply,
        Divide,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        Equal,
        NotEqual,
        Assign,
        Let,
        Var,
        Print,
        Variable,
        Call,
        Get,
        Set,
        Class,
    }
}
