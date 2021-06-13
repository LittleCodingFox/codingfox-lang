using CodingFoxLang.Compiler.Scanner;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Factor()
        {
            var expression = Unary();

            while(Matches(TokenType.Slash, TokenType.Star))
            {
                var op = Previous;
                var right = Unary();

                expression = new Binary(expression, op, right);
            }

            return expression;
        }
    }
}
