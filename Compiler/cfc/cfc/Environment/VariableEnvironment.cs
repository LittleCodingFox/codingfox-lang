using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class VariableEnvironment
    {
        private Dictionary<string, VariableValue> values = new Dictionary<string, VariableValue>();

        public VariableEnvironment parent { get; private set; }

        public VariableEnvironment()
        {
        }

        public VariableEnvironment(VariableEnvironment parent)
        {
            this.parent = parent;
        }

        public bool Exists(string name)
        {
            return values.ContainsKey(name) || (parent?.Exists(name) ?? false);
        }

        public void Assign(Token name, object value)
        {
            if(values.TryGetValue(name.lexeme, out var variableValue))
            {
                variableValue.value = value;

                return;
            }

            if(parent != null)
            {
                parent.Assign(name, value);

                return;
            }

            throw new RuntimeErrorException(name, $"Undefined variable `{name.lexeme}'.");
        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance)?.Assign(name, value);
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

        public VariableValue Get(Token name)
        {
            if(values.TryGetValue(name.lexeme, out var value))
            {
                return value;
            }

            if(parent != null)
            {
                return parent.Get(name);
            }

            throw new RuntimeErrorException(name, $"Undefined variable `{name.lexeme}'.");
        }

        public VariableValue GetAt(int distance, string name)
        {
            var ancestor = Ancestor(distance);

            if(ancestor == null)
            {
                return null;
            }

            return ancestor.values.TryGetValue(name, out var variableValue) ? variableValue : null;
        }

        private VariableEnvironment Ancestor(int distance)
        {
            VariableEnvironment environment = this;

            for(var i = 0; i < distance; i++)
            {
                environment = environment.parent;
            }

            return environment;
        }
    }
}
