using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Scanner
{
    static class Keywords
    {
        private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.And },
            { "class", TokenType.Class },
            { "else", TokenType.Else },
            { "false", TokenType.False },
            { "for", TokenType.For },
            { "func", TokenType.Func },
            { "if", TokenType.If },
            { "nil", TokenType.Nil },
            { "or", TokenType.Or },
            { "print", TokenType.Print },
            { "return", TokenType.Return },
            { "super", TokenType.Super },
            { "this", TokenType.This },
            { "true", TokenType.True },
            { "var", TokenType.Var },
            { "let", TokenType.Let },
            { "while", TokenType.While }
        };

        public static TokenType? Keyword(string identifier)
        {
            if (keywords.TryGetValue(identifier, out var token))
            {
                return token;
            }

            return null;
        }
    }
}
