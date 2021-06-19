using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected variable name.");
            Token type = null;
            IExpression initializer = null;
            List<IStatement> getStatement = null;
            List<IStatement> setStatement = null;

            if (Matches(TokenType.Equal))
            {
                initializer = Expression();

                Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");
            }
            else
            {
                Consume(TokenType.Colon, "Expected variable type.");

                type = Consume(TokenType.Identifier, "Expected variable type.");

                if (Matches(TokenType.Equal))
                {
                    initializer = Expression();

                    Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");
                }
                else if(Matches(TokenType.LeftBrace))
                {
                    while(!Matches(TokenType.RightBrace))
                    {
                        if(Matches(TokenType.Get))
                        {
                            Consume(TokenType.LeftBrace, "Expected start of get accessor.");

                            getStatement = Block();
                        }
                        else if(Matches(TokenType.Set))
                        {
                            Consume(TokenType.LeftBrace, "Expected start of set accessor.");

                            setStatement = Block();
                        }
                    }
                }
                else
                {
                    Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");
                }
            }

            if(setStatement != null && getStatement == null)
            {
                Error?.Invoke(name.line, $"Got set accessor but no get accessor.");

                throw new ParseError();
            }

            return new VariableStatement(name, type, initializer, getStatement, setStatement);
        }
    }
}
