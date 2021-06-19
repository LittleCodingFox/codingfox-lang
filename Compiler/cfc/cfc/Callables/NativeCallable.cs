using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class NativeCallable : ICallable
    {
        public int ParameterCount => parameterCount;

        private int parameterCount = 0;

        public Func<VariableEnvironment, Interpreter, List<object>, object> Action { get; private set; }

        public VariableEnvironment Closure { get; private set; }

        public NativeCallable(VariableEnvironment closure, int parameterCount, Func<VariableEnvironment, Interpreter, List<object>, object> action)
        {
            this.parameterCount = parameterCount;

            Closure = closure;
            Action = action;
        }

        public object Call(Token token, Interpreter interpreter, List<object> arguments, Action<VariableEnvironment> temporariesSetup = null)
        {
            var environment = new VariableEnvironment(Closure);

            temporariesSetup?.Invoke(environment);

            return Action?.Invoke(environment, interpreter, arguments) ?? null;
        }

        public ICallable Bind(object instance)
        {
            var environment = new VariableEnvironment(Closure);

            environment.Set("this", new VariableValue()
            {
                attributes = VariableAttributes.Set,
                value = instance
            });

            return new NativeCallable(environment, parameterCount, Action);
        }
    }
}
