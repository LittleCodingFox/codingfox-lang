using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitStatementExpression(StatementExpression statementExpression)
        {
            Evaluate(statementExpression.expression);

            return null;
        }
    }
}
