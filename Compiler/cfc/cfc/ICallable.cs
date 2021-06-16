using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    interface ICallable
    {
        int ParameterCount { get; }

        object Call(Interpreter interpreter, List<object> arguments);
    }
}
