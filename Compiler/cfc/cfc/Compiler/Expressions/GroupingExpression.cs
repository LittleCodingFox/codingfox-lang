using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitGroupingExpression(GroupingExpression groupingExpression)
        {
            Evaluate(groupingExpression.expression);

            return null;
        }
    }
}
