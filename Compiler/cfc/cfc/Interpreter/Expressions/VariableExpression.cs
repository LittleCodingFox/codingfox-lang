﻿namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitVariableExpression(VariableExpression variableExpression)
        {
            return LookupVariable(variableExpression.name, variableExpression);
        }
    }
}
