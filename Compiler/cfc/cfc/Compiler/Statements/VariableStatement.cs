using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitVariableStatement(VariableStatement statement)
        {
            if (statement.initializer != null)
            {
                Evaluate(statement.initializer);
            }

            //TODO: Get/Set methods
            VMInstruction.Var(vm.activeChunk, statement.name.lexeme, statement.type, statement.initializer);

            return null;
        }
    }
}
