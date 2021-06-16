using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitGetExpression(GetExpression expression)
        {
            var source = Evaluate(expression.source);

            if(source is ScriptedInstance instance)
            {
                return instance.Get(expression.name)?.value ?? null;
            }

            throw new RuntimeErrorException(expression.name, $"Invalid object for property `{expression.name.lexeme}'.");
        }
    }
}
