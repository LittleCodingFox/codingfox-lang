using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Assignment()
        {
            var expression = Equality();

            if(Matches(TokenType.Equal))
            {
                var equals = Previous;
                var value = Assignment();

                if(expression is VariableExpression variable)
                {
                    return new AssignmentExpression(variable.name, value);
                }

                Error(equals.line, "Expected assignment target.");

                throw new ParseError();
            }

            return expression;
        }
    }
}
