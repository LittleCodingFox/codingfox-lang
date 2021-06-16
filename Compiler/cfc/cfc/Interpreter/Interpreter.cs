using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter : IExpressionVisitor, IStatementVisitor
    {
        private VariableEnvironment globalEnvironment = new VariableEnvironment();

        public Action Error;
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
            catch(ParseError)
            {
                Error?.Invoke();
            }
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
                var text = doubleValue.ToString();

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
