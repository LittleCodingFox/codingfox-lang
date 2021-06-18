using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitFunctionStatement(FunctionStatement statement)
        {
            var function = new ScriptedFunction(statement, environment, false);

            environment.Set(statement.name.lexeme, new VariableValue()
            {
                attributes = VariableAttributes.Set,
                value = function
            });

            return null;
        }
    }
}
