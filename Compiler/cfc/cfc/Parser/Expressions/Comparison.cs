using CodingFoxLang.Compiler.Scanner;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Comparison()
        {
            IExpression expression = Term();

            while(Matches(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var op = Previous;
                var right = Term();

                expression = new Binary(expression, op, right);
            }

            return expression;
        }
    }
}
