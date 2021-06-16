using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement Statement()
        {
            if(Matches(TokenType.For))
            {
                return ForStatement();
            }

            if(Matches(TokenType.If))
            {
                return IfStatement();
            }

            if (Matches(TokenType.Print))
            {
                return PrintStatement();
            }

            if(Matches(TokenType.Return))
            {
                return ReturnStatement();
            }

            if(Matches(TokenType.While))
            {
                return WhileStatement();
            }

            if(Matches(TokenType.LeftBrace))
            {
                return new BlockStatement(Block());
            }

            return ExpressionStatement();
        }
    }
}
