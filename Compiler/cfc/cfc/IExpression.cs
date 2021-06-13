namespace CodingFoxLang.Compiler
{
    interface IExpression
    {
        object Accept(IVisitor visitor);
    }
}
