//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace CodingFoxLang.Compiler {
    using CodingFoxLang.Compiler.Scanner;
    
    
    internal class StatementExpression : IStatement {
        
        public IExpression expression;
        
        public StatementExpression(IExpression expression) {
            this.expression = expression;
        }
        
        public object Accept(IStatementVisitor visitor) {
            return visitor.VisitStatementExpression(this);
        }
    }
}
