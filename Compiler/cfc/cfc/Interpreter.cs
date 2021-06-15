using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class Interpreter : IExpressionVisitor,
        IStatementVisitor
    {
        public Action<int, string> Error;
        public Action<RuntimeErrorException> RuntimeError;

        public void Interpret(List<IStatement> statements)
        {
            try
            {
                foreach(var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch(RuntimeErrorException error)
            {
                RuntimeError?.Invoke(error);
            }
            catch(Parser.SyntaxErrorException e)
            {
                Error?.Invoke((e.Data["Token"] as Scanner.Token).line, e.Message);
            }
        }

        public object VisitBinary(Binary binary)
        {
            var left = Evaluate(binary.left);
            var right = Evaluate(binary.right);

            switch(binary.op.type)
            {
                case Scanner.TokenType.Greater:

                    ValidateNumberType(binary.op, left, right);

                    return (double)left > (double)right;

                case Scanner.TokenType.GreaterEqual:

                    ValidateNumberType(binary.op, left, right);

                    return (double)left >= (double)right;

                case Scanner.TokenType.Less:

                    ValidateNumberType(binary.op, left, right);

                    return (double)left < (double)right;

                case Scanner.TokenType.LessEqual:

                    ValidateNumberType(binary.op, left, right);

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

                    throw new RuntimeErrorException(binary.op, "Operands must be two numbers or two strings.");

                case Scanner.TokenType.Minus:

                    ValidateNumberType(binary.op, left, right);

                    return (double)left - (double)right;

                case Scanner.TokenType.Slash:

                    ValidateNumberType(binary.op, left, right);

                    return (double)left / (double)right;

                case Scanner.TokenType.Star:

                    ValidateNumberType(binary.op, left, right);

                    return (double)left + (double)right;
            }

            return null;
        }

        public object VisitGrouping(Grouping grouping)
        {
            return Evaluate(grouping.expression);
        }

        public object VisitLiteral(Literal literal)
        {
            return literal.value;
        }

        public object VisitUnary(Unary unary)
        {
            var right = Evaluate(unary.right);

            switch(unary.op.type)
            {
                case Scanner.TokenType.Bang:
                    return !IsTruth(right);

                case Scanner.TokenType.Minus:
                    return -(double)right;
            }

            return null;
        }

        public object VisitStatementExpression(StatementExpression statementExpression)
        {
            Evaluate(statementExpression.expression);

            return null;
        }

        public object VisitStatementPrint(StatementPrint statementPrint)
        {
            var value = Evaluate(statementPrint.expression);

            Console.WriteLine(Stringify(value));

            return null;
        }

        private void Execute(IStatement statement)
        {
            statement.Accept(this);
        }

        private string Stringify(object o)
        {
            if(o == null)
            {
                return "nil";
            }

            if(o is double doubleValue)
            {
                var text = o.ToString();

                if(text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }

            return o.ToString();
        }

        private void ValidateNumberType(Scanner.Token op, params object[] operands)
        {
            foreach(var operand in operands)
            {
                if(!(operand is double))
                {
                    throw new RuntimeErrorException(op, "Operand must be a number.");
                }
            }
        }

        private bool IsEqual(object a, object b)
        {
            if(a == null && b == null)
            {
                return true;
            }

            if(a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private bool IsTruth(object o)
        {
            if(o == null || !(o is bool boolValue))
            {
                return false;
            }

            return boolValue;
        }

        private object Evaluate(IExpression expression)
        {
            return expression.Accept(this);
        }
    }
}
