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

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new VariableEnvironment(closure);

            for(var i = 0; i < declaration.parameters.Count; i++)
            {
                environment.Set(declaration.parameters[i].lexeme, new VariableValue()
                {
                    value = arguments[i]
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
                value = instance
            });

            return new ScriptedFunction(declaration, environment, isInitializer);
        }
    }
}
