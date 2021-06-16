using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement ExpressionStatement()
        {
            var expression = Expression();

            Consume(TokenType.Semicolon, "Expect `;' after expression.");

            return new StatementExpression(expression);
        }
    }
}
