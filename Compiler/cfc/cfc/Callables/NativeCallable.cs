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

        public Func<VariableEnvironment, List<object>, object> Action { get; private set; }

        public VariableEnvironment Closure { get; private set; }

        public NativeCallable(VariableEnvironment closure, int parameterCount, Func<VariableEnvironment, List<object>, object> action)
        {
            this.parameterCount = parameterCount;

            Closure = closure;
            Action = action;
        }

        public object Call(Token token, List<object> arguments, Action<VariableEnvironment> temporariesSetup = null)
        {
            var environment = new VariableEnvironment(Closure);

            temporariesSetup?.Invoke(environment);

            return Action?.Invoke(environment, arguments) ?? null;
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
