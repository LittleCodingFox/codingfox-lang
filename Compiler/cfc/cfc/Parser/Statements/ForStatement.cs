using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement ForStatement()
        {
            Consume(TokenType.LeftParenthesis, "Expected '(' after 'for'.");

            IStatement initializer;

            if(Matches(TokenType.Semicolon))
            {
                initializer = null;
            }
            else if(Matches(TokenType.Var))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            IExpression condition = null;

            if(!Check(TokenType.Semicolon))
            {
                condition = Expression();
            }

            Consume(TokenType.Semicolon, "Expected ';' after loop condition.");

            IExpression increment = null;

            if(!Check(TokenType.RightParenthesis))
            {
                increment = Expression();
            }

            Consume(TokenType.RightParenthesis, "Expected ')' after 'for' declaration.");

            var body = Statement();

            if(increment != null)
            {
                body = new BlockStatement(new List<IStatement>(new IStatement[] { body, new ExpressionStatement(increment) }));
            }

            if(condition == null)
            {
                condition = new LiteralExpression(true);
            }

            body = new WhileStatement(condition, body);

            if(initializer != null)
            {
                body = new BlockStatement(new List<IStatement>(new IStatement[] { initializer, body }));
            }

            return body;
        }
    }
}
