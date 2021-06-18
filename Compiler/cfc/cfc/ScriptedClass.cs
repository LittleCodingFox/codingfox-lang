﻿using CodingFoxLang.Compiler.Scanner;
using CodingFoxLang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedClass : ICallable
    {
        public string name { get; private set; }

        private Dictionary<string, ScriptedFunction> methods = new Dictionary<string, ScriptedFunction>();
        public Dictionary<string, VariableValue> properties = new Dictionary<string, VariableValue>();
        private ScriptedClass superclass;

        public TypeInfo TypeInfo { get; private set; }

        public ScriptedClass(string name, ScriptedClass superclass, Dictionary<string, ScriptedFunction> methods, Dictionary<string, VariableValue> properties)
        {
            this.superclass = superclass;
            this.name = name;
            this.methods = methods;
            this.properties = properties;

            if(superclass != null)
            {
                foreach (var property in superclass.properties)
                {
                    if (!properties.ContainsKey(property.Key))
                    {
                        properties.Add(property.Key, new VariableValue()
                        {
                            attributes = property.Value.attributes,
                            value = property.Value.value,
                        });
                    }
                }
            }

            var typeInfo = new TypeInfo(name, this, superclass != null ?
                TypeSystem.TypeSystem.FindType(superclass.name) : null, () =>
                {
                    return false;
                }, (a) =>
                {
                    if(!(a is ScriptedInstance instance) || instance.TypeInfo.scriptedClass != this)
                    {
                        return (false, null);
                    }

                    return (true, a);
                });

            TypeInfo = typeInfo;

            TypeSystem.TypeSystem.RegisterType(typeInfo);
        }

        public object Call(Token token, Interpreter interpreter, List<object> arguments)
        {
            var instance = new ScriptedInstance(this);

            var initializer = FindMethod("init");

            if(initializer != null)
            {
                initializer.closure.inInitializer = true;

                initializer.Bind(instance).Call(token, interpreter, arguments);

                initializer.closure.inInitializer = false;
            }

            return instance;
        }

        public ScriptedFunction FindMethod(string name)
        {
            if (methods.TryGetValue(name, out var method))
            {
                return method;
            }

            if(superclass != null)
            {
                return superclass.FindMethod(name);
            }

            return null;
        }

        public VariableValue FindProperty(string name)
        {
            if(properties.TryGetValue(name, out var property))
            {
                return property;
            }

            return null;
        }

        public int ParameterCount
        {
            get
            {
                var initializer = FindMethod("init");

                if(initializer == null)
                {
                    return 0;
                }

                return initializer.ParameterCount;
            }
        }
    }
}
