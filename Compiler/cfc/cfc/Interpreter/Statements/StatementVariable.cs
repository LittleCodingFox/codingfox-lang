using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitStatementVariable(StatementVariable statementVariable)
        {
            object value = null;

            if (statementVariable.initializer != null)
            {
                value = Evaluate(statementVariable.initializer);
            }

            globalEnvironment.Set(statementVariable.name.lexeme, value);

            return null;
        }
    }
}
