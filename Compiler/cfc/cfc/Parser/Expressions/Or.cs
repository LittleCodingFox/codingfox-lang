using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Or()
        {
            var expression = And();

            while(Matches(TokenType.Or))
            {
                var op = Previous;
                var right = And();

                expression = new LogicalExpression(expression, op, right);
            }

            return expression;
        }
    }
}
