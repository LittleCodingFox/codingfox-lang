using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitVariableExpression(VariableExpression variableExpression)
        {
            return globalEnvironment.Get(variableExpression.name)?.value ?? null;
        }
    }
}
