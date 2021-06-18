using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitClassStatement(ClassStatement statement)
        {
            object superclass = null;
            ScriptedClass superClassInstance = null;

            environment.Set(statement.name.lexeme, new VariableValue() { value = null });

            environment = new VariableEnvironment(environment);

            if(statement.superclass != null)
            {
                superclass = Evaluate(statement.superclass);

                if(!(superclass is ScriptedClass))
                {
                    throw new RuntimeErrorException(statement.superclass.name, "Invalid superclass");
                }

                superClassInstance = (ScriptedClass)superclass;
            }

            if(statement.superclass != null)
            {
                environment = new VariableEnvironment(environment);
                environment.Set("super", new VariableValue()
                {
                    attributes = VariableAttributes.ReadOnly | VariableAttributes.Set,
                    value = superclass
                });
            }

            var properties = new Dictionary<string, VariableValue>();

            foreach (var property in statement.properties)
            {
                var exists = environment.Exists(property.name.lexeme);

                VisitVariableStatement(property);

                properties.Add(property.name.lexeme, environment.Get(property.name));

                if (!exists)
                {
                    environment.Remove(property.name.lexeme);
                }
            }

            foreach (var property in statement.readOnlyProperties)
            {
                var exists = environment.Exists(property.name.lexeme);

                VisitLetStatement(property);

                properties.Add(property.name.lexeme, environment.Get(property.name));

                if(!exists)
                {
                    environment.Remove(property.name.lexeme);
                }
            }

            var methods = new Dictionary<string, ScriptedFunction>();

            foreach(var method in statement.methods)
            {
                var function = new ScriptedFunction(method, environment,
                    method.name.lexeme == "init");

                methods.Add(method.name.lexeme, function);
            }

            var scriptedClass = new ScriptedClass(statement.name.lexeme, superClassInstance, methods, properties);

            if(superclass != null)
            {
                environment = environment.parent;
            }

            environment = environment.parent;

            environment.Assign(statement.name, scriptedClass);

            return null;
        }
    }
}
