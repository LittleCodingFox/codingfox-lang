namespace CodingFoxLang.Compiler
{
    interface IStatement
    {
        object Accept(IStatementVisitor visitor);
    }
}
