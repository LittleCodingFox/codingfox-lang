using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression And()
        {
            var expression = Equality();

            while(Matches(TokenType.And))
            {
                var op = Previous;
                var right = Equality();

                expression = new LogicalExpression(expression, op, right);
            }

            return expression;
        }
    }
}
