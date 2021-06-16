﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitAssignmentExpression(AssignmentExpression assignExpression)
        {
            var value = Evaluate(assignExpression.value);

            globalEnvironment.Assign(assignExpression.name, value);

            return value;
        }
    }
}
