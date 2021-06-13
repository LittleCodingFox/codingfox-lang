using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    class SyntaxErrorException : Exception
    {
        private Token token;
        private string message;

        public SyntaxErrorException(Token token, string message)
        {
            this.token = token;
            this.message = message;
        }

        public override string Message
        {
            get
            {
                if (token.type == TokenType.EOF)
                {
                    return $"Syntax Error: End of File reached at line {token.line}";
                }
                else
                {
                    return $"Syntax Error: at `{token.lexeme}' in line {token.line}";
                }
            }
        }

        public override IDictionary Data
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "Token", token },
                    { "Line", token.line },
                    { "Message", message },
                };
            }
        }
    }
}
