using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitReturnStatement(ReturnStatement statement)
        {
            if (statement.value != null)
            {
                Evaluate(statement.value);
            }

            VMInstruction.Return(vm.activeChunk);

            return null;
        }
    }
}
