using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class VariableEnvironment
    {
        private Dictionary<string, VariableValue> values = new Dictionary<string, VariableValue>();

        public void Assign(Token name, object value)
        {
            if(values.TryGetValue(name.lexeme, out var variableValue))
            {
                variableValue.value = value;

                return;
            }

            throw new RuntimeErrorException(name, $"Undefined variable `{name.lexeme}'.");
        }

        public void Set(string name, object value)
        {
            if(!values.TryGetValue(name, out var variableValue))
            {
                variableValue = new VariableValue();

                values.Add(name, variableValue);
            }

            variableValue.value = value;
        }

        public VariableValue Get(Scanner.Token name)
        {
            if(values.TryGetValue(name.lexeme, out var value))
            {
                return value;
            }

            throw new RuntimeErrorException(name, $"Undefined variable `{name.lexeme}'.");
        }
    }
}
