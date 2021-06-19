using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    interface ICallable
    {
        int ParameterCount { get; }

        object Call(Token token, Interpreter interpreter, List<object> arguments, Action<VariableEnvironment> temporariesSetup = null);

        ICallable Bind(object instance);
    }
}
