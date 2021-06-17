using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitGetExpression(GetExpression expression)
        {
            var source = Evaluate(expression.source);

            if(source is ScriptedInstance instance)
            {
                var value = instance.Get(expression.name);

                if (value != null)
                {
                    if(value.attributes.HasFlag(VariableAttributes.ReadOnly))
                    {
                        environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                    }

                    return value.value;
                }
            }

            throw new RuntimeErrorException(expression.name, $"Invalid object for property `{expression.name.lexeme}'.");
        }
    }
}
