using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            Evaluate(unaryExpression.right);

            VMInstruction.Negate(vm.activeChunk);

            return null;
        }
    }
}
