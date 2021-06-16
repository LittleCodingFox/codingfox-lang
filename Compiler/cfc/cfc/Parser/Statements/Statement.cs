using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement Statement()
        {
            if (Matches(TokenType.Print))
            {
                return PrintStatement();
            }

            return ExpressionStatement();
        }
    }
}
