using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class RuntimeErrorException : Exception
    {
        public Token token;
        public string message;

        public RuntimeErrorException(Token token, string message)
        {
            this.token = token;
            this.message = message;
        }

        public override string Message => message;
    }
}
