﻿using CodingFoxLang.Compiler.Scanner;
using CodingFoxLang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedInstance
    {
        private Dictionary<string, VariableValue> properties = new Dictionary<string, VariableValue>();
        private bool isReadOnly = false;
        
        public TypeInfo TypeInfo { get; private set; }
        public VMScriptedClass ScriptedClass { get; private set; }

        public ScriptedInstance(VMScriptedClass scriptedClass)
        {
            ScriptedClass = scriptedClass;
            TypeInfo = scriptedClass.TypeInfo;

            foreach(var property in scriptedClass.properties)
            {
                properties.Add(property.Key, new VariableValue()
                {
                    attributes = property.Value.attributes,
                    value = property.Value.value,
                    owner = new VariableValue()
                    {
                        value = this,
                    }
                });
            }
        }

        public void SetReadOnly()
        {
            isReadOnly = true;

            foreach(var property in properties)
            {
                property.Value.attributes |= VariableAttributes.ReadOnly;
                property.Value.attributes |= VariableAttributes.Set;
            }
        }

        public bool Exists(string name)
        {
            return properties.ContainsKey(name);
        }

        public VariableValue Get(Token name)
        {
            if(properties.TryGetValue(name.lexeme, out var value))
            {
                return value;
            }

            var method = ScriptedClass.FindMethod(name.lexeme);

            if(method != null)
            {
                return new VariableValue()
                {
                    value = method.Bind(this)
                };
            }

            throw new RuntimeErrorException(name, $"Undefined property '{name.lexeme}'.");
        }

        public void Set(Token name, object value, VariableEnvironment environment)
        {
            if (properties.TryGetValue(name.lexeme, out var property))
            {
                if(!environment.inInitializer && (isReadOnly || property.IsLocked))
                {
                    throw new RuntimeErrorException(name, $"Property '{name.lexeme}' is read only and can only be set once.");
                }

                property.value = value;
                property.attributes |= VariableAttributes.Set;

                return;
            }

            if(ScriptedClass.FindMethod(name.lexeme) != null)
            {
                throw new RuntimeErrorException(name, $"Function '{name.lexeme}' can't be assigned.");
            }

            throw new RuntimeErrorException(name, $"Property '{name.lexeme}' not found.");
        }
    }
}
