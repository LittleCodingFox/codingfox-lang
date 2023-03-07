using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedFunction : ICallable
    {
        private bool isInitializer;

        public VariableEnvironment Closure { get; private set; }

        public FunctionStatement Declaration { get; private set; }

        public int ParameterCount => Declaration?.parameters.Count ?? 0;

        public ScriptedFunction(FunctionStatement declaration, VariableEnvironment closure, bool isInitializer)
        {
            foreach(var parameter in declaration.parameters)
            {
                var typeInfo = TypeSystem.TypeSystem.FindType(parameter.Item2.lexeme);

                if (typeInfo == null)
                {
                    throw new RuntimeErrorException(declaration.name, $"Invalid parameter type for `${parameter.Item1.lexeme}'.");
                }
            }

            if(declaration.returnType != null)
            {
                var typeInfo = TypeSystem.TypeSystem.FindType(declaration.returnType.lexeme);

                if (typeInfo == null)
                {
                    throw new RuntimeErrorException(declaration.name, $"Invalid return type `{declaration.returnType.lexeme}'.");
                }
            }

            Closure = closure;
            Declaration = declaration;
            this.isInitializer = isInitializer;
        }

        public object Call(Token token, List<object> arguments, Action<VariableEnvironment> temporariesSetup = null)
        {
            var environment = new VariableEnvironment(Closure);

            for(var i = 0; i < Declaration.parameters.Count; i++)
            {
                var typeInfo = TypeSystem.TypeSystem.FindType(Declaration.parameters[i].Item2.lexeme);

                if(!TypeSystem.TypeSystem.Convert(arguments[i], typeInfo, out var value))
                {
                    throw new RuntimeErrorException(token, $"Invalid value for parameter `{Declaration.parameters[i].Item1.lexeme}'.");
                }

                environment.Set(Declaration.parameters[i].Item1.lexeme, new VariableValue()
                {
                    attributes = VariableAttributes.Set,
                    typeInfo = typeInfo,
                    value = value
                });
            }

            temporariesSetup?.Invoke(environment);

            try
            {
                //TODO
                //interpreter.ExecuteBlock(Declaration.body, environment);
            }
            catch(ReturnException returnValue)
            {
                if(isInitializer)
                {
                    return Closure.GetAt(0, "this");
                }

                var value = returnValue.value;

                if(Declaration.returnType != null)
                {
                    var typeInfo = TypeSystem.TypeSystem.FindType(Declaration.returnType.lexeme);

                    if(typeInfo == null)
                    {
                        throw new RuntimeErrorException(token, $"Unexpected invalid return type for `{Declaration.name.lexeme}'.");
                    }

                    if(!TypeSystem.TypeSystem.Convert(value, typeInfo, out var outValue))
                    {
                        throw new RuntimeErrorException(token, $"Invalid return value on call to `{Declaration.name.lexeme}'.");
                    }

                    return outValue;
                }

                return returnValue.value;
            }

            if(isInitializer)
            {
                return Closure.GetAt(0, "this");
            }

            return null;
        }

        public ICallable Bind(object instance)
        {
            var environment = new VariableEnvironment(Closure);

            environment.Set("this", new VariableValue()
            {
                attributes = VariableAttributes.Set,
                value = instance
            });

            return new ScriptedFunction(Declaration, environment, isInitializer);
        }
    }
}
