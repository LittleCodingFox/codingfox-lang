using CodingFoxLang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CodingFoxLang.Compiler
{
    internal class VirtualMachine
    {
        public const int StackMax = 256;

        public Dictionary<string, VMChunk> chunks = new Dictionary<string, VMChunk>();
        public VMChunk activeChunk;
        public int IP = 0;
        public VariableValue[] stack = new VariableValue[StackMax];
        public int stackTop;
        public VariableEnvironment globalEnvironment = new VariableEnvironment();

        private void Push(VariableValue value)
        {
            stack[stackTop++] = value;
        }

        private VariableValue Pop()
        {
            stackTop--;

            return stack[stackTop];
        }

        private byte ReadByte()
        {
            return activeChunk.code[IP++];
        }

        private VariableValue ReadConstant()
        {
            return activeChunk.constants[ReadByte()];
        }

        public InterpretResult Interpret()
        {
            for(; ; )
            {
                if(IP >= activeChunk.code.Count)
                {
                    return InterpretResult.OK;
                }

                var instruction = ReadByte();

                switch((VMOpcode)instruction)
                {
                    case VMOpcode.Return:

                        Pop();

                        return InterpretResult.OK;

                    case VMOpcode.Constant:

                        {
                            var constant = ReadConstant();

                            Push(constant);
                        }

                        break;

                    case VMOpcode.Negate:
                        {
                            var b = Pop();

                            try
                            {
                                dynamic a = b.value;

                                a = -a;

                                var v = new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = a,
                                    typeInfo = TypeSystem.TypeSystem.FindType(a.GetType()),
                                };

                                Push(v);
                            }
                            catch(Exception e)
                            {
                                return InterpretResult.RuntimeError;
                            }
                        }

                        break;

                    case VMOpcode.Add:

                        {
                            var right = Pop();
                            var left = Pop();

                            try
                            {
                                dynamic a = left.value;
                                dynamic b = right.value;

                                var value = a + b;

                                var v = new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = value,
                                    typeInfo = TypeSystem.TypeSystem.FindType(value.GetType()),
                                };

                                Push(v);
                            }
                            catch (Exception e)
                            {
                                return InterpretResult.RuntimeError;
                            }
                        }

                        break;

                    case VMOpcode.Subtract:


                        {
                            var right = Pop();
                            var left = Pop();

                            try
                            {
                                dynamic a = left.value;
                                dynamic b = right.value;

                                var value = a - b;

                                var v = new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = value,
                                    typeInfo = TypeSystem.TypeSystem.FindType(value.GetType()),
                                };

                                Push(v);
                            }
                            catch (Exception e)
                            {
                                return InterpretResult.RuntimeError;
                            }
                        }

                        break;

                    case VMOpcode.Divide:

                        {
                            var right = Pop();
                            var left = Pop();

                            try
                            {
                                dynamic a = left.value;
                                dynamic b = right.value;

                                var value = a / b;

                                var v = new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = value,
                                    typeInfo = TypeSystem.TypeSystem.FindType(value.GetType()),
                                };

                                Push(v);
                            }
                            catch (Exception e)
                            {
                                return InterpretResult.RuntimeError;
                            }
                        }

                        break;

                    case VMOpcode.Multiply:

                        {
                            var right = Pop();
                            var left = Pop();

                            try
                            {
                                dynamic a = left.value;
                                dynamic b = right.value;

                                var value = a * b;

                                var v = new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = value,
                                    typeInfo = TypeSystem.TypeSystem.FindType(value.GetType()),
                                };

                                Push(v);
                            }
                            catch (Exception e)
                            {
                                return InterpretResult.RuntimeError;
                            }
                        }

                        break;

                    case VMOpcode.Print:

                        {
                            var a = Pop();

                            var value = a.value;

                            if (value is ScriptedInstance instance && instance.ScriptedClass != null)
                            {
                                var method = instance.ScriptedClass.FindMethod("toString");

                                if (method != null && method.ParameterCount == 0)
                                {
                                    value = method.Bind(instance).Call(new Scanner.Token(Scanner.TokenType.Print, "print", null, 0), new List<object>());
                                }
                            }

                            Console.WriteLine(Stringify(value));
                        }

                        break;

                    case VMOpcode.Let:

                        {
                            if(VMInstruction.ReadString(activeChunk, out var name, ref IP) == false ||
                                VMInstruction.ReadBool(activeChunk, out var hasType, ref IP) == false)
                            {
                                return InterpretResult.RuntimeError;
                            }

                            string typeString = null;

                            if(hasType && VMInstruction.ReadString(activeChunk, out typeString, ref IP) == false)
                            {
                                return InterpretResult.RuntimeError;
                            }

                            if(VMInstruction.ReadBool(activeChunk, out var hasInitializer, ref IP) == false)
                            {
                                return InterpretResult.RuntimeError;
                            }

                            VariableValue initializer = hasInitializer ? Pop() : null;

                            var typeInfo = typeString != null ? TypeSystem.TypeSystem.FindType(typeString) : null;

                            if (typeInfo == null && initializer != null)
                            {
                                if (initializer.value is ScriptedInstance scriptedInstance)
                                {
                                    typeInfo = TypeSystem.TypeSystem.FindType(scriptedInstance.ScriptedClass.name);
                                }
                                else
                                {
                                    typeInfo = TypeSystem.TypeSystem.FindType(initializer.value.GetType());
                                }
                            }

                            object outValue = null;

                            if (typeInfo == null ||
                                (typeInfo.type != null && ((initializer != null && initializer.value == null) ||
                                (initializer != null && !TypeSystem.TypeSystem.Convert(initializer.value, typeInfo, out outValue)))) ||
                                (typeInfo.scriptedClass != null && initializer != null && !TypeSystem.TypeSystem.Convert(initializer.value, typeInfo, out outValue)))
                            {
                                return InterpretResult.RuntimeError;
                            }

                            var attributes = VariableAttributes.ReadOnly;

                            if (initializer != null)
                            {
                                attributes |= VariableAttributes.Set;
                            }

                            activeChunk.environment.Set(name, new VariableValue()
                            {
                                attributes = attributes,
                                typeInfo = typeInfo,
                                value = outValue,
                            });
                        }

                        break;

                    case VMOpcode.Variable:

                        {
                            if(VMInstruction.ReadString(activeChunk, out var variableName, ref IP) == false)
                            {
                                return InterpretResult.RuntimeError;
                            }

                            var variable = activeChunk.environment.Get(new Scanner.Token(Scanner.TokenType.Identifier, variableName, variableName, 0));

                            if(variable == null)
                            {
                                return InterpretResult.RuntimeError;
                            }

                            Push(variable);
                        }

                        break;

                    default:

                        return InterpretResult.RuntimeError;
                }
            }
        }

        private string Stringify(object o)
        {
            if (o == null)
            {
                return "nil";
            }

            if (o is double doubleValue)
            {
                var text = doubleValue.ToString();

                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }

            return o.ToString();
        }
    }
}
