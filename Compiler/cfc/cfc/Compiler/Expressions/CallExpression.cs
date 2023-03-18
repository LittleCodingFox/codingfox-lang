namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitCallExpression(CallExpression expression)
        {
            expression.arguments.Reverse();

            foreach (var argument in expression.arguments)
            {
                Evaluate(argument);
            }

            Evaluate(expression.callee);

            VMInstruction.Call(vm.activeChunk, expression.arguments.Count);

            return null;
        }
    }
}
