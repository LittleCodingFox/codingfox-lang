using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitIfStatement(IfStatement statement)
        {
            if(IsTruth(Evaluate(statement.condition)))
            {
                Execute(statement.thenBranch);
            }
            else if(statement.elseBranch != null)
            {
                Execute(statement.elseBranch);
            }

            return null;
        }
    }
}
