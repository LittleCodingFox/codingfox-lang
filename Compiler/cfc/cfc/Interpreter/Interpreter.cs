using CodingFoxLang.Compiler.Scanner;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter : IExpressionVisitor, IStatementVisitor
    {
        private Dictionary<IExpression, int> locals = new Dictionary<IExpression, int>();

        private VariableEnvironment globalEnvironment = new VariableEnvironment();
        public VariableEnvironment environment;

        public Action Error;
        public Action<RuntimeErrorException> RuntimeError;

        public Interpreter()
        {
            environment = globalEnvironment;
        }

        public void RegisterCallable(string name, ICallable callable)
        {
            globalEnvironment.Set(name, new VariableValue()
            {
                value = callable
            });
        }

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

        public void Execute(IStatement statement)
        {
            statement.Accept(this);
        }

        public void ExecuteBlock(List<IStatement> statements,
            VariableEnvironment environment)
        {
            var previous = this.environment;

            try
            {
                this.environment = environment;

                foreach(var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        public void Resolve(IExpression expression, int depth)
        {
            if(!locals.ContainsKey(expression))
            {
                locals.Add(expression, depth);
            }
            else
            {
                locals[expression] = depth;
            }
        }

        private object LookupVariable(Token name, IExpression expression)
        {
            if(locals.TryGetValue(expression, out var distance))
            {
                var value = environment.GetAt(distance, name.lexeme);

                if (value != null)
                {
                    if(value.attributes.HasFlag(VariableAttributes.ReadOnly))
                    {
                        environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                    }

                    return value.value;
                }
            }
            else
            {
                var value = environment.Get(name);

                if (value != null)
                {
                    if (value.attributes.HasFlag(VariableAttributes.ReadOnly))
                    {
                        environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                    }

                    return value.value;
                }
            }

            return null;
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
