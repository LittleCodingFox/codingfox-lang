using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitSetExpression(SetExpression expression)
        {
            var source = Evaluate(expression.source);

            if(source is ScriptedInstance instance)
            {
                var value = Evaluate(expression.value);

                instance.Set(expression.name, value);

                return value;
            }

            throw new RuntimeErrorException(expression.name, $"Invalid object for property `{expression.name.lexeme}'.");
        }
    }
}
