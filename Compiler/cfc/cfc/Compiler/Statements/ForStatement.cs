﻿using System;

namespace CodingFoxLang.Compiler
{
    partial class Compiler
    {
        public object VisitForStatement(ForStatement statement)
        {
            throw new Exception("For Statement should never be run since it is turned into a while statement");
        }
    }
}
