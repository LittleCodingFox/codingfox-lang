using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitLogicalExpression(LogicalExpression logicalExpression)
        {
            var left = Evaluate(logicalExpression.left);

            if(logicalExpression.op.type == TokenType.Or)
            {
                if(IsTruth(left))
                {
                    return true;
                }
            }
            else
            {
                if(!IsTruth(left))
                {
                    return false;
                }
            }

            return Evaluate(logicalExpression.right);
        }
    }
}
