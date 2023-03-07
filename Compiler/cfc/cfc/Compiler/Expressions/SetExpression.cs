using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitSetExpression(SetExpression expression)
        {
            Evaluate(expression.source);
            Evaluate(expression.value);

            VMInstruction.Set(vm.activeChunk, expression.name.lexeme);

            return null;
        }
    }
}
