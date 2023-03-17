using CodingFoxLang.Compiler.Scanner;
using CodingFoxLang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class VMScriptedClass : ICallable
    {
        public string name { get; private set; }

        private Dictionary<string, VMScriptedFunction> methods = new Dictionary<string, VMScriptedFunction>();
        public Dictionary<string, VariableValue> properties = new Dictionary<string, VariableValue>();

        public VMScriptedClass SuperClass { get; private set; }

        public TypeInfo TypeInfo { get; private set; }

        public VMScriptedClass(string name, VMScriptedClass superClass, Dictionary<string, VMScriptedFunction> methods, Dictionary<string, VariableValue> properties)
        {
            this.name = name;
            this.methods = methods;
            this.properties = properties;

            SuperClass = superClass;

            if (superClass != null)
            {
                foreach (var property in superClass.properties)
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

            var typeInfo = new TypeInfo(name, this, superClass != null ?
                TypeSystem.TypeSystem.FindType(superClass.name) : null, () =>
                {
                    return false;
                }, (a) =>
                {
                    if (!(a is ScriptedInstance instance) || instance.TypeInfo.scriptedClass != this)
                    {
                        return (false, null);
                    }

                    return (true, a);
                });

            TypeInfo = typeInfo;

            TypeSystem.TypeSystem.RegisterType(typeInfo);
        }

        public object Call(Token token, List<object> arguments, Action<VariableEnvironment> temporariesSetup = null)
        {
            var instance = new ScriptedInstance(this);

            var initializer = FindMethod("init");

            if (initializer != null)
            {
                initializer.Closure.inInitializer = true;

                initializer.Bind(instance).Call(token, arguments);

                initializer.Closure.inInitializer = false;
            }

            return instance;
        }

        public ICallable Bind(object instance)
        {
            return null;
        }

        public VMScriptedFunction FindMethod(string name)
        {
            if (methods.TryGetValue(name, out var method))
            {
                return method;
            }

            if (SuperClass != null)
            {
                return SuperClass.FindMethod(name);
            }

            return null;
        }

        public VariableValue FindProperty(string name)
        {
            if (properties.TryGetValue(name, out var property))
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

                if (initializer == null)
                {
                    return 0;
                }

                return initializer.ParameterCount;
            }
        }
    }
}
