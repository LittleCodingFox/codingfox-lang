using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitVariableStatement(VariableStatement statement)
        {
            if(environment.Exists(statement.name.lexeme, false))
            {
                throw new RuntimeErrorException(statement.name, $"Variable `{statement.name.lexeme}' already exists.");
            }

            var typeInfo = TypeSystem.TypeSystem.FindType(statement.type.lexeme);

            if (typeInfo == null)
            {
                throw new RuntimeErrorException(statement.type, $"Type `{statement.type.lexeme}' not found.");
            }

            object value = null;

            if (statement.initializer != null)
            {
                value = Evaluate(statement.initializer);
            }

            object outValue = null;

            if ((typeInfo.type != null && ((statement.initializer != null && value == null) || (value != null && !TypeSystem.TypeSystem.Convert(value, typeInfo, out outValue)))) ||
                (typeInfo.scriptedClass != null && value != null && !TypeSystem.TypeSystem.Convert(value, typeInfo, out outValue)))
            {
                throw new RuntimeErrorException(statement.type, $"Invalid value for `{statement.name.lexeme}'.");
            }

            var variableValue = new VariableValue()
            {
                typeInfo = typeInfo,
                value = outValue,
            };

            if(outValue != null)
            {
                variableValue.attributes = VariableAttributes.Set;
            }

            environment.Set(statement.name.lexeme, variableValue);

            return null;
        }
    }
}
