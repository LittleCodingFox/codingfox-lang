using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitGroupingExpression(GroupingExpression groupingExpression)
        {
            return Evaluate(groupingExpression.expression);
        }
    }
}
