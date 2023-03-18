namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitFunctionStatement(FunctionStatement statement)
        {
            HandleFunctionStatement(statement);

            return null;
        }
    }
}
