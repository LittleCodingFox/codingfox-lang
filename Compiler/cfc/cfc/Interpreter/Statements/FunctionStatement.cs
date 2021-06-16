using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitFunctionStatement(FunctionStatement statement)
        {
            var function = new ScriptedFunction(statement, globalEnvironment);

            globalEnvironment.Set(statement.name.lexeme, function);

            return null;
        }
    }
}
