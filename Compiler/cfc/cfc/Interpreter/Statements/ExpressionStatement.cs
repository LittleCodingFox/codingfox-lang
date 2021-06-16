using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitExpressionStatement(ExpressionStatement statement)
        {
            Evaluate(statement.expression);

            return null;
        }
    }
}
