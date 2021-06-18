using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement LetDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected variable name.");

            Consume(TokenType.Colon, "Expected variable type.");

            var type = Consume(TokenType.Identifier, "Expected variable type.");

            IExpression initializer = null;

            if (Matches(TokenType.Equal))
            {
                initializer = Expression();
            }

            Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");

            return new LetStatement(name, type, initializer);
        }
    }
}
