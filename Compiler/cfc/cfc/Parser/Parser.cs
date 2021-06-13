using CodingFoxLang.Compiler.Scanner;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private List<Token> tokens;
        private int current = 0;

        public System.Action<int, string> Error;

        public Token Previous
        {
            get
            {
                return tokens[current - 1];
            }
        }

        public bool EOF
        {
            get
            {
                return Peek.type == TokenType.EOF;
            }
        }

        public Token Peek
        {
            get
            {
                return tokens[current];
            }
        }

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public IExpression Parse()
        {
            try
            {
                return Expression();
            }
            catch (SyntaxErrorException)
            {
                return null;
            }
        }

        private IExpression Expression()
        {
            return Equality();
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            var token = Peek;

            Error(token.line, message);

            throw new SyntaxErrorException(token, message);
        }

        private void Synchronize()
        {
            Advance();

            while(!EOF)
            {
                if(Previous.type == TokenType.Semicolon)
                {
                    return;
                }

                switch(Peek.type)
                {
                    case TokenType.Class:
                    case TokenType.Func:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Return:

                        return;
                }

                Advance();
            }
        }

        private bool Matches(params TokenType[] types)
        {
            foreach(var type in types)
            {
                if(Check(type))
                {
                    Advance();

                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if(EOF)
            {
                return false;
            }

            return Peek.type == type;
        }

        private Token Advance()
        {
            if(!EOF)
            {
                current++;
            }

            return Previous;
        }
    }
}
