using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitWhileStatement(WhileStatement statement)
        {
            while(IsTruth(Evaluate(statement.condition)))
            {
                Execute(statement.body);
            }

            return null;
        }
    }
}
