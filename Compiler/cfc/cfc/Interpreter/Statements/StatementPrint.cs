using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitStatementPrint(StatementPrint statementPrint)
        {
            var value = Evaluate(statementPrint.expression);

            Console.WriteLine(Stringify(value));

            return null;
        }
    }
}
