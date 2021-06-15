namespace CodingFoxLang.Compiler
{
    interface IExpression
    {
        object Accept(IExpressionVisitor visitor);
    }
}
