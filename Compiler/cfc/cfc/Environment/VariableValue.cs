using CodingFoxLang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    [Flags]
    enum VariableAttributes
    {
        None,
        ReadOnly = (1 << 1),
        Set = (1 << 2)
    }

    class VariableValue
    {
        public VariableAttributes attributes = VariableAttributes.None;
        public object value;
        public VariableValue owner;
        public TypeInfo typeInfo;

        public bool IsReadOnly
        {
            get
            {
                return attributes.HasFlag(VariableAttributes.ReadOnly) || (owner?.IsReadOnly ?? false);
            }
        }

        public bool IsLocked
        {
            get
            {
                return (attributes.HasFlag(VariableAttributes.ReadOnly) && attributes.HasFlag(VariableAttributes.Set)) || (owner?.IsLocked ?? false);
            }
        }
    }
}
