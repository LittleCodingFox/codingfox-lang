using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.TypeSystem
{
    class TypeInfo
    {
        public string name;
        public Type type;
        public VMScriptedClass scriptedClass;
        public TypeInfo parent;

        public Func<object> createCallback;
        public Func<object, (bool, object)> convertCallback;

        private Dictionary<string, Func<VariableEnvironment, ICallable>> callables = new Dictionary<string, Func<VariableEnvironment, ICallable>>();

        public TypeInfo(string name, Type type, TypeInfo parent, Func<object> create, Func<object, (bool, object)> convert)
        {
            this.name = name;
            this.type = type;
            this.parent = parent;

            createCallback = create;
            convertCallback = convert;
        }

        public TypeInfo(string name, VMScriptedClass scriptedClass, TypeInfo parent, Func<object> create, Func<object, (bool, object)> convert)
        {
            this.name = name;
            this.scriptedClass = scriptedClass;
            this.parent = parent;

            createCallback = create;
            convertCallback = convert;
        }

        public Func<VariableEnvironment, ICallable> FindCallable(string name)
        {
            return callables.TryGetValue(name, out var callable) ? callable : null;
        }

        public void RegisterCallable(string name, Func<VariableEnvironment, ICallable> callable)
        {
            if(!callables.ContainsKey(name))
            {
                callables.Add(name, callable);
            }
        }
    }
}
