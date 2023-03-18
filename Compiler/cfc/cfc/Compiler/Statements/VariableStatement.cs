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

            VMInstruction.Var(vm.activeChunk, statement.name.lexeme, statement.type,
                statement.initializer != null,
                statement.getStatements != null,
                statement.setStatements != null);

            return null;
        }
    }
}
