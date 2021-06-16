using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private List<IStatement> Block()
        {
            var statements = new List<IStatement>();

            while(!Check(TokenType.RightBrace) && !EOF)
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RightBrace, "Expected '}' after block.");

            return statements;
        }
    }
}
