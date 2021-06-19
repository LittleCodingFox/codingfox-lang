using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ActionCallable2 : ICallable
    {
        public int ParameterCount => 1;

        public Func<VariableEnvironment, object, object> Action { get; private set; }

        public VariableEnvironment Closure { get; private set; }

        public ActionCallable2(VariableEnvironment closure, Func<VariableEnvironment, object, object> action)
        {
            Closure = closure;
            Action = action;
        }

        public object Call(Token token, Interpreter interpreter, List<object> arguments)
        {
            var environment = new VariableEnvironment(Closure);

            return Action?.Invoke(environment, arguments[0]) ?? null;
        }

        public ICallable Bind(object instance)
        {
            var environment = new VariableEnvironment(Closure);

            environment.Set("this", new VariableValue()
            {
                attributes = VariableAttributes.Set,
                value = instance
            });

            return new ActionCallable2(environment, Action);
        }
    }
}
