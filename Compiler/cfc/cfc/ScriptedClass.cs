using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    class ScriptedClass : ICallable
    {
        public string name { get; private set; }

        private Dictionary<string, ScriptedFunction> methods = new Dictionary<string, ScriptedFunction>();
        private ScriptedClass superclass;

        public ScriptedClass(string name, ScriptedClass superclass, Dictionary<string, ScriptedFunction> methods)
        {
            this.superclass = superclass;
            this.name = name;
            this.methods = methods;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new ScriptedInstance(this);

            var initializer = FindMethod("init");

            if(initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
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
