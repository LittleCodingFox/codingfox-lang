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
    
    
    internal class BlockStatement : IStatement {
        
        public List<IStatement> statements;
        
        public BlockStatement(List<IStatement> statements) {
            this.statements = statements;
        }
        
        public object Accept(IStatementVisitor visitor) {
            return visitor.VisitBlockStatement(this);
        }
    }
}
