using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ActionCallable : ICallable
    {
        public int ParameterCount => 0;

        public Func<VariableEnvironment, object> Action { get; private set; }

        public VariableEnvironment Closure { get; private set; }

        public ActionCallable(VariableEnvironment closure, Func<VariableEnvironment, object> action)
        {
            Closure = closure;
            Action = action;
        }

        public object Call(Token token, Interpreter interpreter, List<object> arguments)
        {
            var environment = new VariableEnvironment(Closure);

            return Action?.Invoke(environment) ?? null;
        }

        public ICallable Bind(object instance)
        {
            var environment = new VariableEnvironment(Closure);

            environment.Set("this", new VariableValue()
            {
                attributes = VariableAttributes.Set,
                value = instance
            });

            return new ActionCallable(environment, Action);
        }
    }
}
