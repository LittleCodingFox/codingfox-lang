using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitSuperExpression(SuperExpression expression)
        {
            VMInstruction.Super(vm.activeChunk, expression);

            return null;
        }
    }
}
