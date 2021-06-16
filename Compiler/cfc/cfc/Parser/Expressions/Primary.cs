using CodingFoxLang.Compiler.Scanner;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Primary()
        {
            if(Matches(TokenType.False))
            {
                return new LiteralExpression(false);
            }

            if(Matches(TokenType.True))
            {
                return new LiteralExpression(true);
            }

            if(Matches(TokenType.Nil))
            {
                return new LiteralExpression(null);
            }

            if(Matches(TokenType.Number, TokenType.String))
            {
                return new LiteralExpression(Previous.literal);
            }

            if(Matches(TokenType.Identifier))
            {
                return new VariableExpression(Previous);
            }

            if(Matches(TokenType.LeftParenthesis))
            {
                var expression = Expression();

                Consume(TokenType.RightParenthesis, "Expect ')' after expression.");

                return new GroupingExpression(expression);
            }

            var token = Peek;

            Error(token.line, "Expected expression");

            throw new ParseError();
        }
    }
}
