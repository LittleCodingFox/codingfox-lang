using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitVariableStatement(VariableStatement statement)
        {
            if(globalEnvironment.Exists(statement.name.lexeme))
            {
                throw new RuntimeErrorException(statement.name, $"Variable `{statement.name.lexeme}' already exists.");
            }

            object value = null;

            if (statement.initializer != null)
            {
                value = Evaluate(statement.initializer);
            }

            globalEnvironment.Set(statement.name.lexeme, value);

            return null;
        }
    }
}
