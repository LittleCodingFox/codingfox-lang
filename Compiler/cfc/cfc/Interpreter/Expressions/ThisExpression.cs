using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitThisExpression(ThisExpression expression)
        {
            return LookupVariable(expression.keyword, expression);
        }
    }
}
