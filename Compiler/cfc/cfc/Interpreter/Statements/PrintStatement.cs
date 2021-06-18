using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitPrintStatement(PrintStatement statement)
        {
            var value = Evaluate(statement.expression);

            if(value is ScriptedInstance instance && instance.ScriptedClass != null)
            {
                var method = instance.ScriptedClass.FindMethod("toString");

                if(method != null && method.ParameterCount == 0)
                {
                    value = method.Bind(instance).Call(statement.token, this, new List<object>());
                }
            }

            Console.WriteLine(Stringify(value));

            return null;
        }
    }
}
