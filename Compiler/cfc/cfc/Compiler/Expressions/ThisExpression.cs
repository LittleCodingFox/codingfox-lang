using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitThisExpression(ThisExpression expression)
        {
            VMInstruction.This(vm.activeChunk);

            return null;
        }
    }
}
