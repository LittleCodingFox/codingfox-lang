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
            var expression = Or();

            if(Matches(TokenType.Equal))
            {
                var equals = Previous;
                var value = Assignment();

                if(expression is VariableExpression variable)
                {
                    return new AssignmentExpression(variable.name, value);
                }
                else if(expression is GetExpression getExpression)
                {
                    return new SetExpression(getExpression.source, getExpression.name, value);
                }

                Error(equals.line, "Expected assignment target.");

                throw new ParseError();
            }

            return expression;
        }
    }
}
