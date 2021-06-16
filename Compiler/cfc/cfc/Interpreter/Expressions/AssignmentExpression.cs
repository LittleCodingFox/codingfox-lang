using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitAssignmentExpression(AssignmentExpression assignExpression)
        {
            var value = Evaluate(assignExpression.value);

            if(locals.TryGetValue(assignExpression, out var distance))
            {
                globalEnvironment.AssignAt(distance, assignExpression.name, value);
            }
            else
            {
                globalEnvironment.Assign(assignExpression.name, value);
            }

            return value;
        }
    }
}
