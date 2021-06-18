using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedFunction : ICallable
    {
        private FunctionStatement declaration;
        private bool isInitializer;

        public VariableEnvironment closure;

        public int ParameterCount => declaration?.parameters.Count ?? 0;

        public ScriptedFunction(FunctionStatement declaration, VariableEnvironment closure, bool isInitializer)
        {
            this.closure = closure;
            this.declaration = declaration;
            this.isInitializer = isInitializer;
        }

        public object Call(Token token, Interpreter interpreter, List<object> arguments)
        {
            var environment = new VariableEnvironment(closure);

            for(var i = 0; i < declaration.parameters.Count; i++)
            {
                var typeInfo = TypeSystem.TypeSystem.FindType(declaration.parameters[i].Item2.lexeme);

                if(!TypeSystem.TypeSystem.Convert(arguments[i], typeInfo, out var value))
                {
                    throw new RuntimeErrorException(token, $"Invalid value for parameter `{declaration.parameters[i].Item1.lexeme}'.");
                }

                environment.Set(declaration.parameters[i].Item1.lexeme, new VariableValue()
                {
                    attributes = VariableAttributes.Set,
                    typeInfo = typeInfo,
                    value = value
                });
            }

            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch(ReturnException returnValue)
            {
                if(isInitializer)
                {
                    return closure.GetAt(0, "this");
                }

                return returnValue.value;
            }

            if(isInitializer)
            {
                return closure.GetAt(0, "this");
            }

            return null;
        }

        public ScriptedFunction Bind(ScriptedInstance instance)
        {
            var environment = new VariableEnvironment(closure);
            environment.Set("this", new VariableValue()
            {
                attributes = VariableAttributes.Set,
                value = instance
            });

            return new ScriptedFunction(declaration, environment, isInitializer);
        }
    }
}
