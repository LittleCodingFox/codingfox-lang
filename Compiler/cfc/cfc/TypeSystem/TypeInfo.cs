using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.TypeSystem
{
    class TypeInfo
    {
        public string name;
        public Type type;
        public ScriptedClass scriptedClass;
        public TypeInfo parent;

        public Func<object> createCallback;
        public Func<object, (bool, object)> convertCallback;

        public TypeInfo(string name, Type type, TypeInfo parent, Func<object> create, Func<object, (bool, object)> convert)
        {
            this.name = name;
            this.type = type;
            this.parent = parent;

            createCallback = create;
            convertCallback = convert;
        }

        public TypeInfo(string name, ScriptedClass scriptedClass, TypeInfo parent, Func<object> create, Func<object, (bool, object)> convert)
        {
            this.name = name;
            this.scriptedClass = scriptedClass;
            this.parent = parent;

            createCallback = create;
            convertCallback = convert;
        }
    }
}
