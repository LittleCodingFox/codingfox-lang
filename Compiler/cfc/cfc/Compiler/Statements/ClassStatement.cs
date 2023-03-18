namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitClassStatement(ClassStatement statement)
        {
            VMInstruction.Class(vm.activeChunk, statement.name.lexeme, statement.superclass,
                statement.properties, statement.readOnlyProperties, statement.methods);

            if (statement.superclass != null)
            {
                VMInstruction.WriteString(vm.activeChunk, statement.superclass.name.lexeme);
            }

            foreach (var property in statement.properties)
            {
                if (property.initializer != null)
                {
                    Evaluate(property.initializer);
                }
                else
                {
                    VMInstruction.NoOp(vm.activeChunk);
                }

                VMInstruction.SerializeVar(vm.activeChunk, property.name.lexeme, property.type, property.initializer != null,
                    property.getStatements != null, property.setStatements != null);

                if(property.getStatements != null)
                {
                    CreatePropertyMethod($"{property.name}_get", property.type.lexeme, property.getStatements);
                }

                if(property.setStatements != null)
                {
                    CreatePropertyMethod($"{property.name}_set", property.type.lexeme, property.setStatements);
                }
            }

            foreach (var property in statement.readOnlyProperties)
            {
                if (property.initializer != null)
                {
                    Evaluate(property.initializer);
                }
                else
                {
                    VMInstruction.NoOp(vm.activeChunk);
                }

                VMInstruction.SerializeVar(vm.activeChunk, property.name.lexeme, property.type, property.initializer != null, false, false);
            }

            foreach (var method in statement.methods)
            {
                HandleFunctionStatement(method);
            }

            return null;
        }
    }
}
