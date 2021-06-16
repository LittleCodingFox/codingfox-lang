using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitLiteralExpression(LiteralExpression literalExpression)
        {
            return literalExpression.value;
        }
    }
}
