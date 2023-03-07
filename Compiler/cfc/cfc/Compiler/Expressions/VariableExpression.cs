namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitVariableExpression(VariableExpression variableExpression)
        {
            VMInstruction.Variable(vm.activeChunk, variableExpression.name.lexeme);

            return null;
        }
    }
}
