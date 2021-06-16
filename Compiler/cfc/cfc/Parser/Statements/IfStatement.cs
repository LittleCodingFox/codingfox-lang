using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement IfStatement()
        {
            Consume(TokenType.LeftParenthesis, "Expected '(' after 'if'.");

            var condition = Expression();

            Consume(TokenType.RightParenthesis, "Expected ')' after if condition.");

            var thenBranch = Statement();
            IStatement elseBranch = null;

            if(Matches(TokenType.Else))
            {
                elseBranch = Statement();
            }

            return new IfStatement(condition, thenBranch, elseBranch);
        }
    }
}
