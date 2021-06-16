using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedInstance
    {
        private ScriptedClass scriptedClass;
        private Dictionary<string, VariableValue> fields = new Dictionary<string, VariableValue>();

        public ScriptedInstance(ScriptedClass scriptedClass)
        {
            this.scriptedClass = scriptedClass;
        }

        public VariableValue Get(Token name)
        {
            if(fields.TryGetValue(name.lexeme, out var value))
            {
                return value;
            }

            var method = scriptedClass.FindMethod(name.lexeme);

            if(method != null)
            {
                return new VariableValue()
                {
                    value = method.Bind(this)
                };
            }

            throw new RuntimeErrorException(name, $"Undefined property '{name.lexeme}'.");
        }

        public void Set(Token name, object value)
        {
            if(!fields.TryGetValue(name.lexeme, out var variableValue))
            {
                variableValue = new VariableValue()
                {
                    value = value,
                };

                fields.Add(name.lexeme, variableValue);

                return;
            }

            fields[name.lexeme].value = variableValue;
        }
    }
}
