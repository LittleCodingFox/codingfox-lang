namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitAssignmentExpression(AssignmentExpression assignExpression)
        {
            var name = assignExpression.name.lexeme;

            Evaluate(assignExpression.value);

            VMInstruction.Assign(vm.activeChunk, name);

            return null;
        }
    }
}
