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

            while(!Check(TokenType.RightBrace) && !EOF)
            {
                methods.Add(Function("method"));
            }

            Consume(TokenType.RightBrace, "Expected '}' after class body.");

            return new ClassStatement(name, superclass, methods);
        }
    }
}
