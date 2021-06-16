using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Call()
        {
            var expression = Primary();

            while(true)
            {
                if(Matches(TokenType.LeftParenthesis))
                {
                    expression = FinishCall(expression);
                }
                else if(Matches(TokenType.Dot))
                {
                    var name = Consume(TokenType.Identifier, "Expected property name after '.'.");

                    expression = new GetExpression(expression, name);
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        private IExpression FinishCall(IExpression callee)
        {
            var arguments = new List<IExpression>();

            if(!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if(arguments.Count >= 255)
                    {
                        Error?.Invoke(Peek.line, "Exceeded maximum argument count of 255");

                        throw new ParseError();
                    }

                    arguments.Add(Expression());
                }
                while (Matches(TokenType.Comma));
            }

            var parenthesis = Consume(TokenType.RightParenthesis, "Expected ')' after arguments.");

            return new CallExpression(callee, parenthesis, arguments);
        }
    }
}
