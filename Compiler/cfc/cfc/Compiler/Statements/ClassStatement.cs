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

                VMInstruction.SerializeVar(vm.activeChunk, property.name.lexeme, property.type, property.initializer);
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

                VMInstruction.SerializeVar(vm.activeChunk, property.name.lexeme, property.type, property.initializer);
            }

            foreach (var method in statement.methods)
            {
                HandleFunctionStatement(method);
            }

            return null;
        }
    }
}
