using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ActionCallable2 : ICallable
    {
        public int ParameterCount => 1;

        public Func<object, object> action;

        public object Call(Token token, Interpreter interpreter, List<object> arguments)
        {
            return action?.Invoke(arguments[0]) ?? null;
        }
    }
}
