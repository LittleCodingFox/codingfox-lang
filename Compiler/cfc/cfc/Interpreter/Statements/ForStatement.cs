using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitForStatement(ForStatement statement)
        {
            throw new Exception("For Statement should never be run since it is turned into a while statement");
        }
    }
}
