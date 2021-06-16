using CodingFoxLang.Compiler.Scanner;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Term()
        {
            var expression = Factor();

            while(Matches(TokenType.Minus, TokenType.Plus))
            {
                var op = Previous;
                var right = Factor();

                expression = new BinaryExpression(expression, op, right);
            }

            return expression;
        }
    }
}
