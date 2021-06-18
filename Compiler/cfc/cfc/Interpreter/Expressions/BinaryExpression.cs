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

                    {
                        TypeSystem.TypeInfo typeInfo;

                        if (left is ScriptedInstance instance)
                        {
                            typeInfo = instance.TypeInfo;
                        }
                        else
                        {
                            typeInfo = TypeSystem.TypeSystem.FindType(left.GetType());
                        }

                        if (typeInfo == null || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var a) || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var b))
                        {
                            throw new RuntimeErrorException(binaryExpression.op, "Operand must be a number.");
                        }

                        if (a is IComparable ac && b is IComparable bc)
                        {
                            return ac.CompareTo(bc) > 0;
                        }
                    }

                    return false;

                case Scanner.TokenType.GreaterEqual:

                    ValidateNumberType(binaryExpression.op, left, right);

                    {
                        TypeSystem.TypeInfo typeInfo;

                        if (left is ScriptedInstance instance)
                        {
                            typeInfo = instance.TypeInfo;
                        }
                        else
                        {
                            typeInfo = TypeSystem.TypeSystem.FindType(left.GetType());
                        }

                        if (typeInfo == null || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var a) || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var b))
                        {
                            throw new RuntimeErrorException(binaryExpression.op, "Operand must be a number.");
                        }

                        if (a is IComparable ac && b is IComparable bc)
                        {
                            return ac.CompareTo(bc) >= 0;
                        }
                    }

                    return false;

                case Scanner.TokenType.Less:

                    ValidateNumberType(binaryExpression.op, left, right);

                    {
                        TypeSystem.TypeInfo typeInfo;

                        if (left is ScriptedInstance instance)
                        {
                            typeInfo = instance.TypeInfo;
                        }
                        else
                        {
                            typeInfo = TypeSystem.TypeSystem.FindType(left.GetType());
                        }

                        if (typeInfo == null || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var a) || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var b))
                        {
                            throw new RuntimeErrorException(binaryExpression.op, "Operand must be a number.");
                        }

                        if (a is IComparable ac && b is IComparable bc)
                        {
                            return ac.CompareTo(bc) < 0;
                        }
                    }

                    return false;

                case Scanner.TokenType.LessEqual:

                    ValidateNumberType(binaryExpression.op, left, right);

                    {
                        TypeSystem.TypeInfo typeInfo;

                        if (left is ScriptedInstance instance)
                        {
                            typeInfo = instance.TypeInfo;
                        }
                        else
                        {
                            typeInfo = TypeSystem.TypeSystem.FindType(left.GetType());
                        }

                        if (typeInfo == null || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var a) || !TypeSystem.TypeSystem.Convert(left, typeInfo, out var b))
                        {
                            throw new RuntimeErrorException(binaryExpression.op, "Operand must be a number.");
                        }

                        if (a is IComparable ac && b is IComparable bc)
                        {
                            return ac.CompareTo(bc) <= 0;
                        }
                    }

                    return false;

                case Scanner.TokenType.BangEqual:
                    return !IsEqual(left, right);

                case Scanner.TokenType.EqualEqual:
                    return IsEqual(left, right);

                case Scanner.TokenType.Plus:

                    try
                    {
                        dynamic a = left;
                        dynamic b = right;

                        return a + b;
                    }
                    catch (System.Exception)
                    {
                        throw new RuntimeErrorException(binaryExpression.op, $"Invalid operand types.");
                    }

                case Scanner.TokenType.Minus:

                    ValidateNumberType(binaryExpression.op, left, right);

                    try
                    {
                        dynamic a = left;
                        dynamic b = right;

                        return a - b;
                    }
                    catch (System.Exception)
                    {
                        throw new RuntimeErrorException(binaryExpression.op, $"Invalid operand types.");
                    }

                case Scanner.TokenType.Slash:

                    ValidateNumberType(binaryExpression.op, left, right);

                    try
                    {
                        dynamic a = left;
                        dynamic b = right;

                        return a / b;
                    }
                    catch (System.Exception)
                    {
                        throw new RuntimeErrorException(binaryExpression.op, $"Invalid operand types.");
                    }

                case Scanner.TokenType.Star:

                    ValidateNumberType(binaryExpression.op, left, right);

                    try
                    {
                        dynamic a = left;
                        dynamic b = right;

                        return a * b;
                    }
                    catch(System.Exception)
                    {
                        throw new RuntimeErrorException(binaryExpression.op, $"Invalid operand types.");
                    }
            }

            return null;
        }
    }
}
