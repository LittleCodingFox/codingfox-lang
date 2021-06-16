using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            var right = Evaluate(unaryExpression.right);

            switch (unaryExpression.op.type)
            {
                case Scanner.TokenType.Bang:
                    return !IsTruth(right);

                case Scanner.TokenType.Minus:
                    return -(double)right;
            }

            return null;
        }
    }
}
