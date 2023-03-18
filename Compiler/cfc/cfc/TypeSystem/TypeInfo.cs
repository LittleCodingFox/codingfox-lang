using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.TypeSystem
{
    class TypeInfo
    {
        public enum BinaryOperation
        {
            Add,
            Subtract,
            Divide,
            Multiply,
            Unary,
            Greater,
            GreaterEqual,
            Less,
            LessEqual,
            Equal,
            Different,
        }

        public delegate object CreateCallback();
        public delegate (bool, object) ConvertCallback(object source);
        public delegate (bool, object) BinaryOperationCallback(object left, object right, BinaryOperation op);

        public string name;
        public Type type;
        public VMScriptedClass scriptedClass;
        public TypeInfo parent;

        public CreateCallback createCallback;
        public ConvertCallback convertCallback;
        public BinaryOperationCallback binaryOp;

        private Dictionary<string, Func<VariableEnvironment, ICallable>> callables = new Dictionary<string, Func<VariableEnvironment, ICallable>>();

        public TypeInfo(string name, Type type, TypeInfo parent, CreateCallback create, ConvertCallback convert, BinaryOperationCallback binOp)
        {
            this.name = name;
            this.type = type;
            this.parent = parent;

            createCallback = create;
            convertCallback = convert;
            binaryOp = binOp;
        }

        public TypeInfo(string name, VMScriptedClass scriptedClass, TypeInfo parent, CreateCallback create, ConvertCallback convert)
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
