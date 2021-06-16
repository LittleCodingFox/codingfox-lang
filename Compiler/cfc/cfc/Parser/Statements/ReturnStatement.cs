using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement ReturnStatement()
        {
            var keyword = Previous;
            IExpression value = null;

            if(!Check(TokenType.Semicolon))
            {
                value = Expression();
            }

            Consume(TokenType.Semicolon, "Expected ';' after return value.");

            return new ReturnStatement(keyword, value);
        }
    }
}
