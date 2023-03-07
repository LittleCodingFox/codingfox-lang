using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitLetStatement(LetStatement statement)
        {
            if(statement.initializer != null)
            {
                Evaluate(statement.initializer);
            }

            VMInstruction.Let(vm.activeChunk, statement.name.lexeme, statement.type, statement.initializer);

            return null;
        }
    }
}
