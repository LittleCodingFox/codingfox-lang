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
    
    
    internal class VariableStatement : IStatement {
        
        public Token name;
        
        public Token type;
        
        public IExpression initializer;
        
        public List<IStatement> getStatements;
        
        public List<IStatement> setStatements;
        
        public VariableStatement(Token name, Token type, IExpression initializer, List<IStatement> getStatements, List<IStatement> setStatements) {
            this.name = name;
            this.type = type;
            this.initializer = initializer;
            this.getStatements = getStatements;
            this.setStatements = setStatements;
        }
        
        public object Accept(IStatementVisitor visitor) {
            return visitor.VisitVariableStatement(this);
        }
    }
}
