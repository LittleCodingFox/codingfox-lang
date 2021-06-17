using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement ClassDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected class name");

            VariableExpression superclass = null;

            if(Matches(TokenType.Colon))
            {
                Consume(TokenType.Identifier, "Expected superclass name.");

                superclass = new VariableExpression(Previous);
            }

            Consume(TokenType.LeftBrace, "Expected '{' before class body.");

            var methods = new List<FunctionStatement>();
            var properties = new List<VariableStatement>();
            var readOnlyProperties = new List<LetStatement>();

            while(!Check(TokenType.RightBrace) && !EOF)
            {
                if(Matches(TokenType.Func) || (Check(TokenType.Identifier) && Peek.lexeme == "init"))
                {
                    methods.Add(Function("method"));
                }
                else if(Matches(TokenType.Var))
                {
                    var variableStatement = (VariableStatement)VarDeclaration();

                    if(variableStatement != null)
                    {
                        properties.Add(variableStatement);
                    }
                }
                else if (Matches(TokenType.Let))
                {
                    var letStatement = (LetStatement)LetDeclaration();

                    if (letStatement != null)
                    {
                        readOnlyProperties.Add(letStatement);
                    }
                }
                else
                {
                    Error?.Invoke(Previous.line, "Expected variable or function declaration in class.");

                    throw new ParseError();
                }
            }

            Consume(TokenType.RightBrace, "Expected '}' after class body.");

            return new ClassStatement(name, superclass, methods, properties, readOnlyProperties);
        }
    }
}
