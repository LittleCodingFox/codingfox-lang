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
                return new Literal(false);
            }

            if(Matches(TokenType.True))
            {
                return new Literal(true);
            }

            if(Matches(TokenType.Nil))
            {
                return new Literal(null);
            }

            if(Matches(TokenType.Number, TokenType.String))
            {
                return new Literal(Previous.literal);
            }

            if(Matches(TokenType.LeftParenthesis))
            {
                var expression = Expression();

                Consume(TokenType.RightParenthesis, "Expect ')' after expression.");

                return new Grouping(expression);
            }

            var token = Peek;

            Error(token.line, "Expected expression");

            throw new SyntaxErrorException(token, "Expected expression");
        }
    }
}
