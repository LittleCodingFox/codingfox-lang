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
    using System.Collections.Generic;
    
    
    internal class ForStatement : IStatement {
        
        public IExpression initializer;
        
        public IExpression condition;
        
        public IExpression increment;
        
        public IStatement body;
        
        public ForStatement(IExpression initializer, IExpression condition, IExpression increment, IStatement body) {
            this.initializer = initializer;
            this.condition = condition;
            this.increment = increment;
            this.body = body;
        }
        
        public object Accept(IStatementVisitor visitor) {
            return visitor.VisitForStatement(this);
        }
    }
}
