﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitClassStatement(ClassStatement statement)
        {
            object superclass = null;
            ScriptedClass superClassInstance = null;

            if(statement.superclass != null)
            {
                superclass = Evaluate(statement.superclass);

                if(!(superclass is ScriptedClass))
                {
                    throw new RuntimeErrorException(statement.superclass.name, "Invalid superclass");
                }

                superClassInstance = (ScriptedClass)superclass;
            }

            environment.Set(statement.name.lexeme, null);

            if(statement.superclass != null)
            {
                environment = new VariableEnvironment(environment);
                environment.Set("super", superclass);
            }

            var methods = new Dictionary<string, ScriptedFunction>();

            foreach(var method in statement.methods)
            {
                var function = new ScriptedFunction(method, environment,
                    method.name.lexeme == "init");

                methods.Add(method.name.lexeme, function);
            }

            var scriptedClass = new ScriptedClass(statement.name.lexeme, superClassInstance, methods);

            if(superclass != null)
            {
                environment = environment.parent;
            }

            environment.Assign(statement.name, scriptedClass);

            return null;
        }
    }
}
