using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitSetExpression(SetExpression expression)
        {
            var source = Evaluate(expression.source);

            if(source is ScriptedInstance instance)
            {
                var value = Evaluate(expression.value);

                var target = instance.Get(expression.name);

                if(target == null)
                {
                    throw new RuntimeErrorException(expression.name, $"Property `{expression.name.lexeme}' not found.");
                }

                if(target.value is ScriptedProperty property)
                {
                    if(property.SetFunction == null)
                    {
                        throw new RuntimeErrorException(expression.name, $"Property `{expression.name.lexeme}' is read-only.");
                    }

                    property.SetFunction.Bind(source).Call(expression.name, new List<object>(), (env) =>
                    {
                        env.Set("value", new VariableValue()
                        {
                            attributes = VariableAttributes.ReadOnly | VariableAttributes.Set,
                            value = value,
                        });
                    });

                    return null;
                }

                instance.Set(expression.name, value, environment);

                return value;
            }

            throw new RuntimeErrorException(expression.name, $"Invalid object for property `{expression.name.lexeme}'.");
        }
    }
}
