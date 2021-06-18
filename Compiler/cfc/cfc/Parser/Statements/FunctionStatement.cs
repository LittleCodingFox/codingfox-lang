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

            var parameters = new List<(Token, Token)>();

            if(!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error?.Invoke(Peek.line, "Exceeded maximum amount of arguments: 255");

                        throw new ParseError();
                    }

                    var identifier = Consume(TokenType.Identifier, "Expected parameter name.");

                    Consume(TokenType.Colon, "Expected parameter type.");

                    var type = Consume(TokenType.Identifier, "Expected parameter type.");

                    parameters.Add((identifier, type));
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
