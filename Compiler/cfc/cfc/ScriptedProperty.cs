using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedProperty
    {
        public ICallable GetFunction = null;

        public ICallable SetFunction = null;

        public ScriptedProperty(ICallable getFunction, ICallable setFunction)
        {
            GetFunction = getFunction;
            SetFunction = setFunction;
        }
    }
}
