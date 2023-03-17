using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitBlockStatement(BlockStatement statement)
        {
            Compile(statement.statements);

            return null;
        }
    }
}
