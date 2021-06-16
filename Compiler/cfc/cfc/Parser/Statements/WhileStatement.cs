using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement WhileStatement()
        {
            Consume(TokenType.LeftParenthesis, "Expected '(' after 'while'.");

            var condition = Expression();

            Consume(TokenType.RightParenthesis, "Expected ')' after condition.");

            var body = Statement();

            return new WhileStatement(condition, body);
        }
    }
}
