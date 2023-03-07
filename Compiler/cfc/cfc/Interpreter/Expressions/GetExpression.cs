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

                    if(value.value is ScriptedProperty property)
                    {
                        return property.GetFunction.Bind(instance).Call(expression.name, new List<object>());
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

                    if(result.ParameterCount == 0)
                    {
                        var outValue = result.Bind(source).Call(expression.name, new List<object>());

                        if (outValue is ScriptedProperty property)
                        {
                            if (property.GetFunction == null)
                            {
                                throw new RuntimeErrorException(expression.name, $"Property `{expression.name.lexeme}' is missing a getter.");
                            }

                            return property.GetFunction.Bind(source).Call(expression.name, new List<object>());
                        }
                        else
                        {
                            return outValue;
                        }
                    }
                    else
                    {
                        return result.Bind(source);
                    }
                }
            }

            throw new RuntimeErrorException(expression.name, $"Property `{expression.name.lexeme}' not found.");
        }
    }
}
