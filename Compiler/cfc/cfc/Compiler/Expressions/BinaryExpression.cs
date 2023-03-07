namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            Evaluate(binaryExpression.left);
            Evaluate(binaryExpression.right);

            switch (binaryExpression.op.type)
            {
                case Scanner.TokenType.Greater:

                    VMInstruction.Greater(vm.activeChunk);

                    break;

                case Scanner.TokenType.GreaterEqual:

                    VMInstruction.GreaterEqual(vm.activeChunk);

                    break;

                case Scanner.TokenType.Less:

                    VMInstruction.Less(vm.activeChunk);

                    break;

                case Scanner.TokenType.LessEqual:

                    VMInstruction.LessEqual(vm.activeChunk);

                    break;

                case Scanner.TokenType.BangEqual:

                    VMInstruction.NotEqual(vm.activeChunk);

                    break;

                case Scanner.TokenType.EqualEqual:

                    VMInstruction.Equal(vm.activeChunk);

                    break;

                case Scanner.TokenType.Plus:

                    VMInstruction.Add(vm.activeChunk);

                    break;

                case Scanner.TokenType.Minus:

                    VMInstruction.Subtract(vm.activeChunk);

                    break;

                case Scanner.TokenType.Slash:

                    VMInstruction.Divide(vm.activeChunk);

                    break;

                case Scanner.TokenType.Star:

                    VMInstruction.Multiply(vm.activeChunk);

                    break;
            }

            return null;
        }
    }
}
