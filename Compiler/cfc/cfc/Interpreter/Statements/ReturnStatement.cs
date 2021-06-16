using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitReturnStatement(ReturnStatement statement)
        {
            object value = null;

            if(statement.value != null)
            {
                value = Evaluate(statement.value);
            }

            throw new ReturnException(value);
        }
    }
}
