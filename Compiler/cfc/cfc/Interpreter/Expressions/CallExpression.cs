using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitCallExpression(CallExpression expression)
        {
            var value = Evaluate(expression.callee);

            if (value is ICallable callable)
            {
                var arguments = new List<object>();

                foreach (var argument in expression.arguments)
                {
                    arguments.Add(Evaluate(argument));
                }

                if(arguments.Count != callable.ParameterCount)
                {
                    throw new RuntimeErrorException(expression.parenthesis, $"Expected {callable.ParameterCount} arguments but got {arguments.Count}.");
                }

                return callable.Call(expression.parenthesis, arguments);
            }

            throw new RuntimeErrorException(expression.parenthesis, "Caller is not a function or class");
        }
    }
}
