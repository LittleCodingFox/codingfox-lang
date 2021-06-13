using CodingFoxLang.Compiler.Scanner;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IExpression Equality()
        {
            IExpression expression = Comparison();

            while (Matches(TokenType.BangEqual, TokenType.EqualEqual))
            {
                var op = Previous;
                var right = Comparison();

                expression = new Binary(expression, op, right);
            }

            return expression;
        }
    }
}
