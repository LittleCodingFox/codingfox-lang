using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitPrintStatement(PrintStatement statement)
        {
            Evaluate(statement.expression);

            VMInstruction.WriteChunk(vm.activeChunk, (byte)VMOpcode.Print);

            return null;
        }
    }
}
