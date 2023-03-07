using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitClassStatement(ClassStatement statement)
        {
            VMInstruction.Class(vm.activeChunk, statement.name.lexeme, statement.superclass,
                statement.properties, statement.readOnlyProperties, statement.methods);

            foreach (var property in statement.properties)
            {
                if (property.initializer != null)
                {
                    Evaluate(property.initializer);
                }

                VMInstruction.SerializeVar(vm.activeChunk, property.name.lexeme, property.type, property.initializer);
            }

            foreach (var property in statement.readOnlyProperties)
            {
                if (property.initializer != null)
                {
                    Evaluate(property.initializer);
                }

                VMInstruction.SerializeVar(vm.activeChunk, property.name.lexeme, property.type, property.initializer);
            }

            foreach (var method in statement.methods)
            {
                //TODO
            }

            if (statement.superclass != null)
            {
                Evaluate(statement.superclass);
            }

            return null;
        }
    }
}
