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
            TypeSystem.TypeInfo typeInfo = null;

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

                typeInfo = TypeSystem.TypeSystem.FindType(instance.ScriptedClass.name);
            }
            else if(source != null)
            {
                typeInfo = TypeSystem.TypeSystem.FindType(source.GetType());
            }

            if(typeInfo != null)
            {
                var callableFunc = typeInfo.FindCallable(expression.name.lexeme);

                if(callableFunc != null)
                {
                    var result = callableFunc(environment);

                    return result.Bind(source);
                }
            }

            throw new RuntimeErrorException(expression.name, $"Property `{expression.name.lexeme}' not found.");
        }
    }
}
