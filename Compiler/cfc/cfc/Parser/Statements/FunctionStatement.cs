using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private FunctionStatement Function(string kind)
        {
            Token name = Consume(TokenType.Identifier, $"Expected {kind} name.");

            Consume(TokenType.LeftParenthesis, $"Expected '(' after {kind} name.");

            var parameters = new List<Token>();

            if(!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error?.Invoke(Peek.line, "Exceeded maximum amount of arguments: 255");

                        throw new ParseError();
                    }

                    parameters.Add(Consume(TokenType.Identifier, "Expected parameter name."));
                }
                while (Matches(TokenType.Comma));
            }

            Consume(TokenType.RightParenthesis, "Expected ')' after parameters.");

            Consume(TokenType.LeftBrace, $"Expected '{{' before {kind} body.");

            var body = Block();

            return new FunctionStatement(name, parameters, body);
        }
    }
}
