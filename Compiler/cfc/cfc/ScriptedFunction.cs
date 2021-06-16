using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedFunction : ICallable
    {
        private FunctionStatement declaration;
        private VariableEnvironment closure;

        public int ParameterCount => declaration?.parameters.Count ?? 0;

        public ScriptedFunction(FunctionStatement declaration, VariableEnvironment closure)
        {
            this.closure = closure;
            this.declaration = declaration;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new VariableEnvironment(closure);

            for(var i = 0; i < declaration.parameters.Count; i++)
            {
                environment.Set(declaration.parameters[i].lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch(ReturnException returnValue)
            {
                return returnValue.value;
            }

            return null;
        }
    }
}
