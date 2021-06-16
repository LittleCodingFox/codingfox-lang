using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Parser
{
    partial class Parser
    {
        private IStatement Declaration()
        {
            try
            {
                if(Matches(TokenType.Func))
                {
                    return Function("function");
                }

                if (Matches(TokenType.Var))
                {
                    return VarDeclaration();
                }

                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();

                return null;
            }
        }
    }
}
