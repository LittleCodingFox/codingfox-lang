using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitPrintStatement(PrintStatement statement)
        {
            var value = Evaluate(statement.expression);

            Console.WriteLine(Stringify(value));

            return null;
        }
    }
}
