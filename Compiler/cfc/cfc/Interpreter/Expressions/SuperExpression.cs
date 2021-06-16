using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitSuperExpression(SuperExpression expression)
        {
            if(locals.TryGetValue(expression, out var distance))
            {
                var superclass = (ScriptedClass)environment.GetAt(distance, "super")?.value ?? null;
                var thisObject = (ScriptedInstance)environment.GetAt(distance - 1, "this")?.value ?? null;

                var method = superclass.FindMethod(expression.method.lexeme);

                if(method == null)
                {
                    throw new RuntimeErrorException(expression.method, $"Undefined property '{expression.method.lexeme}'.");
                }

                return method.Bind(thisObject);
            }

            return null;
        }
    }
}
