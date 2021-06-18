using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ActionCallable : ICallable
    {
        public int ParameterCount => 0;

        public Func<object> action;

        public object Call(Token token, Interpreter interpreter, List<object> arguments)
        {
            return action?.Invoke() ?? null;
        }
    }
}
