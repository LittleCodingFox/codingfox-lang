using System.Collections.Generic;

namespace CodingFoxLang.Compiler
{
    internal class VMChunk
    {
        public string name;
        public List<byte> code = new List<byte>();
        public List<VariableValue> constants = new List<VariableValue>();
        public Dictionary<string, int> locals = new Dictionary<string, int>();
        public VariableEnvironment environment;

        public VMChunk(VariableEnvironment parent)
        {
            environment = new VariableEnvironment(parent);
        }
    }
}
