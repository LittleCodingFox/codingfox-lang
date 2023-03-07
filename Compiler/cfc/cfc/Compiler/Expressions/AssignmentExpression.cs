namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitAssignmentExpression(AssignmentExpression assignExpression)
        {
            var name = assignExpression.name.lexeme;

            Evaluate(assignExpression.value);

            if (vm.activeChunk.locals.TryGetValue(name, out var distance))
            {
                VMInstruction.AssignAt(vm.activeChunk, name, distance);
            }
            else
            {
                VMInstruction.Assign(vm.activeChunk, name);
            }

            return null;
        }
    }
}
