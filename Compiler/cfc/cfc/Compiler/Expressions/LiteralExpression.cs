namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitLiteralExpression(LiteralExpression literalExpression)
        {
            var v = new VariableValue()
            {
                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                value = literalExpression.value,
            };

            if(v.value != null)
            {
                v.typeInfo = TypeSystem.TypeSystem.FindType(v.value.GetType());
            }

            var constant = VMInstruction.AddConstant(vm.activeChunk, v);

            VMInstruction.Constant(vm.activeChunk, constant);

            return literalExpression.value;
        }
    }
}
