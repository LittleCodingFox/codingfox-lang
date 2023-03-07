using System;
using System.Linq;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitCallExpression(CallExpression expression)
        {
            foreach (var argument in expression.arguments.Select(x => x).Reverse())
            {
                Evaluate(argument);
            }

            Evaluate(expression.callee);

            VMInstruction.Call(vm.activeChunk, expression.arguments.Count);

            return null;
        }
    }
}
