using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ReturnException : Exception
    {
        public object value;

        public ReturnException(object value)
        {
            this.value = value;
        }
    }
}
