using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitBlockStatement(BlockStatement statement)
        {
            ExecuteBlock(statement.statements, new VariableEnvironment(globalEnvironment));

            return null;
        }
    }
}
