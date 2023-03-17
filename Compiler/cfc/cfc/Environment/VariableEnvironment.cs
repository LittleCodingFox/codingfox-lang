using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class VariableEnvironment
    {
        public enum WriteProtection
        {
            None,
            ReadOnly
        }

        private Dictionary<string, VariableValue> values = new Dictionary<string, VariableValue>();

        public WriteProtection writeProtection = WriteProtection.None;

        public bool inInitializer = false;

        public VariableEnvironment parent { get; private set; }

        public VariableEnvironment()
        {
        }

        public VariableEnvironment(VariableEnvironment parent)
        {
            this.parent = parent;
            writeProtection = parent.writeProtection;
            inInitializer = parent.inInitializer;
        }

        public bool Exists(string name, bool recursive = true)
        {
            return values.ContainsKey(name) || (recursive && (parent?.Exists(name) ?? false));
        }

        public void Remove(string name)
        {
            if(values.ContainsKey(name))
            {
                values.Remove(name);

                return;
            }

            parent?.Remove(name);
        }

        public void Assign(Token name, object value)
        {
            if(values.TryGetValue(name.lexeme, out var variableValue))
            {
                if(!inInitializer &&
                    (writeProtection == WriteProtection.ReadOnly ||
                    (variableValue.attributes.HasFlag(VariableAttributes.ReadOnly) && variableValue.attributes.HasFlag(VariableAttributes.Set))))
                {
                    throw new RuntimeErrorException(name, $"Cannot assign value to readonly variable `{name.lexeme}': You can only set their value once.");
                }

                var typeInfo = variableValue.typeInfo;

                if(typeInfo != null)
                {
                    if ((typeInfo.type != null && (value == null || (value != null && value.GetType() != typeInfo.type))) ||
                        (typeInfo.scriptedClass != null && value != null &&
                        (!(value is ScriptedInstance instance) || instance.TypeInfo.scriptedClass.name != typeInfo.scriptedClass.name)))
                    {
                        throw new RuntimeErrorException(name, $"Invalid value for `{name.lexeme}'.");
                    }
                }

                variableValue.attributes |= VariableAttributes.Set;
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

        public void Set(string name, VariableValue value)
        {
            if(values.TryGetValue(name, out var variableValue))
            {
                variableValue.attributes = value.attributes;
                variableValue.typeInfo = value.typeInfo;
                variableValue.value = value;
            }
            else
            {
                values.Add(name, value);
            }
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
