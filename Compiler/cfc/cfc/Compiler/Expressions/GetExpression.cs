using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitGetExpression(GetExpression expression)
        {
            Evaluate(expression.source);

            VMInstruction.Get(vm.activeChunk, expression.name.lexeme);

            return null;
        }
    }
}
