using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitLetStatement(LetStatement statement)
        {
            if (environment.Exists(statement.name.lexeme))
            {
                throw new RuntimeErrorException(statement.name, $"Variable `{statement.name.lexeme}' already exists.");
            }

            object value = null;

            if (statement.initializer != null)
            {
                value = Evaluate(statement.initializer);
            }

            var attributes = VariableAttributes.ReadOnly;

            if(value != null)
            {
                attributes |= VariableAttributes.Set;
            }

            environment.Set(statement.name.lexeme, new VariableValue()
            {
                value = value,
                attributes = attributes,
            });

            return null;
        }
    }
}
