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
    
    
    internal class LiteralExpression : IExpression {
        
        public object value;
        
        public LiteralExpression(object value) {
            this.value = value;
        }
        
        public object Accept(IExpressionVisitor visitor) {
            return visitor.VisitLiteralExpression(this);
        }
    }
}
