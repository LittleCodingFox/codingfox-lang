using CodingFoxLang.Compiler.Scanner;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Unary()
        {
            if(Matches(TokenType.Bang, TokenType.Minus))
            {
                var op = Previous;
                var right = Unary();

                return new Unary(op, right);
            }

            return Primary();
        }
    }
}
