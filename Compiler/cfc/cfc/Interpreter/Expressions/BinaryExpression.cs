using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            var left = Evaluate(binaryExpression.left);
            var right = Evaluate(binaryExpression.right);

            switch (binaryExpression.op.type)
            {
                case Scanner.TokenType.Greater:

                    ValidateNumberType(binaryExpression.op, left, right);

                    return (double)left > (double)right;

                case Scanner.TokenType.GreaterEqual:

                    ValidateNumberType(binaryExpression.op, left, right);

                    return (double)left >= (double)right;

                case Scanner.TokenType.Less:

                    ValidateNumberType(binaryExpression.op, left, right);

                    return (double)left < (double)right;

                case Scanner.TokenType.LessEqual:

                    ValidateNumberType(binaryExpression.op, left, right);

                    return (double)left <= (double)right;

                case Scanner.TokenType.BangEqual:
                    return !IsEqual(left, right);

                case Scanner.TokenType.EqualEqual:
                    return IsEqual(left, right);

                case Scanner.TokenType.Plus:
                    {
                        if (left is double lhs && right is double rhs)
                        {
                            return lhs + rhs;
                        }
                    }

                    {
                        if (left is string lhs && right is string rhs)
                        {
                            return lhs + rhs;
                        }
                    }

                    throw new RuntimeErrorException(binaryExpression.op, "Operands must be two numbers or two strings.");

                case Scanner.TokenType.Minus:

                    ValidateNumberType(binaryExpression.op, left, right);

                    return (double)left - (double)right;

                case Scanner.TokenType.Slash:

                    ValidateNumberType(binaryExpression.op, left, right);

                    return (double)left / (double)right;

                case Scanner.TokenType.Star:

                    ValidateNumberType(binaryExpression.op, left, right);

                    return (double)left * (double)right;
            }

            return null;
        }
    }
}
