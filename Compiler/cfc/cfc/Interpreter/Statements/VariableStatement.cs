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

            var typeInfo = statement.type != null ? TypeSystem.TypeSystem.FindType(statement.type.lexeme) : null;

            object value = null;

            if (statement.initializer != null)
            {
                value = Evaluate(statement.initializer);
            }

            if(typeInfo == null && value != null)
            {
                if (value is ScriptedInstance scriptedInstance)
                {
                    typeInfo = TypeSystem.TypeSystem.FindType(scriptedInstance.ScriptedClass.name);
                }
                else
                {
                    typeInfo = TypeSystem.TypeSystem.FindType(value.GetType());
                }
            }

            object outValue = null;

            if(typeInfo == null || (typeInfo.type != null && ((statement.initializer != null && value == null) || (value != null && !TypeSystem.TypeSystem.Convert(value, typeInfo, out outValue)))) ||
                (typeInfo.scriptedClass != null && value != null && !TypeSystem.TypeSystem.Convert(value, typeInfo, out outValue)))
            {
                throw new RuntimeErrorException(statement.name, $"Invalid value for `{statement.name.lexeme}'.");
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
