using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement PrintStatement()
        {
            var value = Expression();

            var token = Consume(TokenType.Semicolon, "Expect `;' after expression.");

            return new PrintStatement(token, value);
        }
    }
}
