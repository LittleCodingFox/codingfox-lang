using CodingFoxLang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;

namespace CodingFoxLang.Compiler
{
    internal class VirtualMachine
    {
        public const int StackMax = 256;

        public class StackFrame
        {
            public VMChunk chunk;
            public int IP = 0;
        }

        public Dictionary<string, VMChunk> chunks = new Dictionary<string, VMChunk>();
        public VMChunk activeChunk;
        public VariableValue[] stack = new VariableValue[StackMax];
        public int stackTop;
        public VariableEnvironment globalEnvironment = new VariableEnvironment();

        private int bindCounter = 0;

        public List<StackFrame> callStack = new List<StackFrame>();

        public StackFrame CurrentCall => callStack.Count > 0 ? callStack[callStack.Count - 1] : null;

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
            return CurrentCall.chunk.code[CurrentCall.IP++];
        }

        private VariableValue ReadConstant()
        {
            return CurrentCall.chunk.constants[ReadByte()];
        }

        public void Interpret()
        {
            for(; ; )
            {
                try
                {
                    ExecuteOne();
                }
                catch(RuntimeErrorException)
                {
                    return;
                }
                catch(ReturnException)
                {
                    return;
                }
                catch(ExitException)
                {
                    return;
                }
                catch(Exception)
                {
                    return;
                }
            }
        }

        internal void ExecuteOne()
        {
            if (CurrentCall.IP >= CurrentCall.chunk.code.Count)
            {
                callStack.Remove(CurrentCall);

                if(callStack.Count == 0)
                {
                    throw new ExitException();
                }

                return;
            }

            var instruction = ReadByte();

            try
            {
                switch ((VMOpcode)instruction)
                {
                    case VMOpcode.Return:

                        {
                            var result = Pop();

                            throw new ReturnException(result?.value);
                        }

                    case VMOpcode.Constant:

                        {
                            var constant = ReadConstant();

                            Push(constant);
                        }

                        break;

                    case VMOpcode.Negate:
                        {
                            var b = Pop();

                            if(b.value == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   "Attempt to negate null value");
                            }

                            if (b.value is ScriptedProperty property)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = property.GetFunction.Bind(This).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    b = vt;
                                }
                                else
                                {
                                    b = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            var typeInfo = TypeSystem.TypeSystem.FindType(b.value.GetType());

                            var result = typeInfo?.binaryOp?.Invoke(b.value, null, TypeInfo.BinaryOperation.Unary);

                            if ((result?.Item1 ?? false) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Value of type {typeInfo.name} can't be negated");
                            }

                            var v = new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = result.Value.Item2,
                                typeInfo = TypeSystem.TypeSystem.FindType(result.Value.GetType()),
                            };

                            Push(v);
                        }

                        break;

                    case VMOpcode.Add:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (right.value == null || left.value == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   "Attempt to add null value");
                            }

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if(This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if(t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            var typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());

                            var result = typeInfo?.binaryOp?.Invoke(left.value, right.value, TypeInfo.BinaryOperation.Add);

                            if ((result?.Item1 ?? false) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Value of type {typeInfo.name} can't be added");
                            }

                            var v = new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = result.Value.Item2,
                                typeInfo = TypeSystem.TypeSystem.FindType(result.Value.GetType()),
                            };

                            Push(v);
                        }

                        break;

                    case VMOpcode.Subtract:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (right.value == null || left.value == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   "Attempt to subtract null value");
                            }

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            var typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());

                            var result = typeInfo?.binaryOp?.Invoke(left.value, right.value, TypeInfo.BinaryOperation.Subtract);

                            if ((result?.Item1 ?? false) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Value of type {typeInfo.name} can't be subtract");
                            }

                            var v = new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = result.Value.Item2,
                                typeInfo = TypeSystem.TypeSystem.FindType(result.Value.GetType()),
                            };

                            Push(v);
                        }

                        break;

                    case VMOpcode.Divide:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (right.value == null || left.value == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   "Attempt to divide null value");
                            }

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            var typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());

                            var result = typeInfo?.binaryOp?.Invoke(left.value, right.value, TypeInfo.BinaryOperation.Divide);

                            if ((result?.Item1 ?? false) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Value of type {typeInfo.name} can't be divided");
                            }

                            var v = new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = result.Value.Item2,
                                typeInfo = TypeSystem.TypeSystem.FindType(result.Value.GetType()),
                            };

                            Push(v);
                        }

                        break;

                    case VMOpcode.Multiply:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (right.value == null || left.value == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   "Attempt to multiply null value");
                            }

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            var typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());

                            var result = typeInfo?.binaryOp?.Invoke(left.value, right.value, TypeInfo.BinaryOperation.Multiply);

                            if ((result?.Item1 ?? false) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Value of type {typeInfo.name} can't be added");
                            }

                            var v = new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = result.Value.Item2,
                                typeInfo = TypeSystem.TypeSystem.FindType(result.Value.GetType()),
                            };

                            Push(v);
                        }

                        break;

                    case VMOpcode.Greater:

                        {
                            var right = Pop();
                            var left = Pop();

                            TypeInfo typeInfo;

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (left.value is ScriptedInstance instance)
                            {
                                typeInfo = instance.TypeInfo;
                            }
                            else
                            {
                                typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());
                            }

                            if (typeInfo == null ||
                                !TypeSystem.TypeSystem.Convert(left.value, typeInfo, out var a) ||
                                !TypeSystem.TypeSystem.Convert(right.value, typeInfo, out var b))
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Invalid operand values");
                            }

                            if (a is IComparable ac && b is IComparable bc)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set,
                                    value = ac.CompareTo(bc) > 0,
                                });

                                return;
                            }

                            Push(new VariableValue()
                            {
                                attributes = VariableAttributes.Set,
                                value = false,
                            });
                        }

                        break;

                    case VMOpcode.GreaterEqual:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            TypeInfo typeInfo;

                            if (left.value is ScriptedInstance instance)
                            {
                                typeInfo = instance.TypeInfo;
                            }
                            else
                            {
                                typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());
                            }

                            if (typeInfo == null ||
                                !TypeSystem.TypeSystem.Convert(left.value, typeInfo, out var a) ||
                                !TypeSystem.TypeSystem.Convert(right.value, typeInfo, out var b))
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Invalid operand values");
                            }

                            if (a is IComparable ac && b is IComparable bc)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set,
                                    value = ac.CompareTo(bc) >= 0,
                                });

                                return;
                            }

                            Push(new VariableValue()
                            {
                                attributes = VariableAttributes.Set,
                                value = false,
                            });
                        }

                        break;

                    case VMOpcode.Less:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            TypeInfo typeInfo;

                            if (left.value is ScriptedInstance instance)
                            {
                                typeInfo = instance.TypeInfo;
                            }
                            else
                            {
                                typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());
                            }

                            if (typeInfo == null ||
                                !TypeSystem.TypeSystem.Convert(left.value, typeInfo, out var a) ||
                                !TypeSystem.TypeSystem.Convert(right.value, typeInfo, out var b))
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Invalid operand values");
                            }

                            if (a is IComparable ac && b is IComparable bc)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set,
                                    value = ac.CompareTo(bc) < 0,
                                });

                                return;
                            }

                            Push(new VariableValue()
                            {
                                attributes = VariableAttributes.Set,
                                value = false,
                            });
                        }

                        break;

                    case VMOpcode.LessEqual:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            TypeInfo typeInfo;

                            if (left.value is ScriptedInstance instance)
                            {
                                typeInfo = instance.TypeInfo;
                            }
                            else
                            {
                                typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());
                            }

                            if (typeInfo == null ||
                                !TypeSystem.TypeSystem.Convert(left.value, typeInfo, out var a) ||
                                !TypeSystem.TypeSystem.Convert(right.value, typeInfo, out var b))
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Invalid operand values");
                            }

                            if (a is IComparable ac && b is IComparable bc)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set,
                                    value = ac.CompareTo(bc) <= 0,
                                });

                                return;
                            }

                            Push(new VariableValue()
                            {
                                attributes = VariableAttributes.Set,
                                value = false,
                            });
                        }

                        break;

                    case VMOpcode.Equal:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value == null && left.value == null)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = true,
                                });
                            }
                            else if(right.value == null || left.value == null)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = false,
                                });
                            }

                            var typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());

                            var result = typeInfo?.binaryOp?.Invoke(left.value, right.value, TypeInfo.BinaryOperation.Add);

                            if (result?.Item1 == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Value of type {typeInfo.name} can't be compared");
                            }

                            var v = new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = result.Value.Item2,
                                typeInfo = TypeSystem.TypeSystem.FindType(result.Value.GetType()),
                            };

                            Push(v);
                        }

                        break;

                    case VMOpcode.NotEqual:

                        {
                            var right = Pop();
                            var left = Pop();

                            if (left.value is ScriptedProperty leftProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = leftProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    left = vt;
                                }
                                else
                                {
                                    left = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value is ScriptedProperty rightProperty)
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                       "Attempt to add property with no instance");
                                }

                                var t = rightProperty.GetFunction.Bind(This.value).Call(new Scanner.Token(Scanner.TokenType.Class, "", "", 0), new List<object>());

                                if (t is VariableValue vt)
                                {
                                    right = vt;
                                }
                                else
                                {
                                    right = new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = t,
                                    };
                                }
                            }

                            if (right.value == null && left.value == null)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = false,
                                });
                            }
                            else if (right.value == null || left.value == null)
                            {
                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                    value = true,
                                });
                            }

                            var typeInfo = TypeSystem.TypeSystem.FindType(left.value.GetType());

                            var result = typeInfo?.binaryOp?.Invoke(left.value, right.value, TypeInfo.BinaryOperation.Add);

                            if (result?.Item1 == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   $"Value of type {typeInfo.name} can't be compared");
                            }

                            var v = new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = result.Value.Item2,
                                typeInfo = TypeSystem.TypeSystem.FindType(result.Value.GetType()),
                            };

                            Push(v);
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

                    case VMOpcode.Assign:

                        {
                            var value = Pop();

                            if (VMInstruction.ReadString(CurrentCall.chunk, out var name, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "assign", null, 0),
                                    $"Failed to read data");
                            }

                            CurrentCall.chunk.environment.Assign(new Scanner.Token(Scanner.TokenType.Identifier, name, name, 0), value.value);
                        }

                        break;

                    case VMOpcode.Let:

                        {
                            if (VMInstruction.ReadString(CurrentCall.chunk, out var name, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadBool(CurrentCall.chunk, out var hasType, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "let", null, 0),
                                    $"Failed to read data");
                            }

                            string typeString = null;

                            if (hasType && VMInstruction.ReadString(CurrentCall.chunk, out typeString, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "let", null, 0),
                                    $"Failed to read data");
                            }

                            if (VMInstruction.ReadBool(CurrentCall.chunk, out var hasInitializer, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "let", null, 0),
                                    $"Failed to read data");
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
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "let", null, 0),
                                    $"Variable {name} has invalid initialization");
                            }

                            var attributes = VariableAttributes.ReadOnly;

                            if (initializer != null)
                            {
                                attributes |= VariableAttributes.Set;
                            }

                            CurrentCall.chunk.environment.Set(name, new VariableValue()
                            {
                                attributes = attributes,
                                typeInfo = typeInfo,
                                value = outValue,
                            });
                        }

                        break;

                    case VMOpcode.Var:

                        {
                            if(VMInstruction.DeserializeVar(CurrentCall.chunk, out var name, out var typeString,
                                out var hasInitializer, out var hasGetter, out var hasSetter, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "var", null, 0),
                                    $"Failed to read data");
                            }

                            VariableValue initializer = hasInitializer ? Pop() : null;

                            var typeInfo = typeString != null ? TypeSystem.TypeSystem.FindType(typeString) : null;

                            if(typeInfo == null && typeString != null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Var, "var", null, 0),
                                    $"Invalid variable type {typeString} for {name}");
                            }

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
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "var", null, 0),
                                    $"Variable {name} has invalid initialization");
                            }

                            VMScriptedFunction getter = null;
                            VMScriptedFunction setter = null;

                            if(hasGetter)
                            {
                                _ = VMInstruction.ReadChunk(CurrentCall.chunk, ref CurrentCall.IP);

                                if (VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                }

                                string returnType = null;

                                if (hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                }

                                if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                                }

                                var arguments = new Dictionary<string, string>();

                                for (var j = 0; j < argumentCount; j++)
                                {
                                    if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    arguments.Add(argumentName, argumentType);
                                }

                                getter = new VMScriptedFunction(this, methodName, CurrentCall.chunk.environment, arguments, returnType, functionChunk, false);
                            }

                            if(hasSetter)
                            {
                                _ = VMInstruction.ReadChunk(CurrentCall.chunk, ref CurrentCall.IP);

                                if (VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                }

                                string returnType = null;

                                if (hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                }

                                if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                                }

                                var arguments = new Dictionary<string, string>();

                                for (var j = 0; j < argumentCount; j++)
                                {
                                    if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    arguments.Add(argumentName, argumentType);
                                }

                                setter = new VMScriptedFunction(this, methodName, CurrentCall.chunk.environment, arguments, returnType, functionChunk, false);
                            }

                            if (hasGetter || hasSetter)
                            {
                                outValue = new ScriptedProperty(getter, setter);
                            }

                            var attributes = VariableAttributes.None;

                            if (initializer != null)
                            {
                                attributes |= VariableAttributes.Set;
                            }

                            CurrentCall.chunk.environment.Set(name, new VariableValue()
                            {
                                attributes = attributes,
                                typeInfo = typeInfo,
                                value = outValue,
                            });
                        }

                        break;

                    case VMOpcode.Variable:

                        {
                            if (VMInstruction.ReadString(CurrentCall.chunk, out var variableName, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "variable", null, 0),
                                    $"Failed to read data");
                            }

                            var token = new Scanner.Token(Scanner.TokenType.Identifier, variableName, variableName, 0);

                            VariableValue variable = null;

                            if(CurrentCall.chunk.environment.Exists(variableName, false))
                            {
                                variable = CurrentCall.chunk.environment.Get(token);
                            }

                            if (variable == null && CurrentCall.chunk.environment.Exists("this"))
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This != null && This.value is ScriptedInstance instance && instance.Exists(variableName))
                                {
                                    variable = instance.Get(token);

                                    /*
                                    if (variable != null && variable.value is ScriptedProperty property)
                                    {
                                        if (property.GetFunction == null)
                                        {
                                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "variable", null, 0),
                                                $"Property {variableName} is write-only");
                                        }

                                        Push(new VariableValue()
                                        {
                                            attributes = VariableAttributes.Set,
                                            value = property.GetFunction.Bind(instance).Call(token, new List<object>())
                                        });

                                        return;
                                    }
                                    */
                                }
                            }

                            if (variable == null)
                            {
                                variable = CurrentCall.chunk.environment.Get(token);
                            }

                            if (variable == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "variable", null, 0),
                                    $"Variable {variableName} is not valid");
                            }

                            Push(variable);
                        }

                        break;

                    case VMOpcode.Call:

                        {
                            if (VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "call", null, 0),
                                    $"Failed to read data");
                            }

                            var callee = Pop();

                            var arguments = new List<object>();

                            for (var i = 0; i < argumentCount; i++)
                            {
                                var v = Pop();

                                arguments.Add(v?.value);
                            }

                            if (callee.value is ICallable callable)
                            {
                                if (callable.ParameterCount != argumentCount)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "call", null, 0),
                                        $"Invalid parameter count: {argumentCount}; Should be: {callable.ParameterCount}");
                                }

                                var result = callable.Call(new Scanner.Token(Scanner.TokenType.Identifier, "", "", 0), arguments);

                                Push(new VariableValue()
                                {
                                    attributes = VariableAttributes.Set,
                                    value = result,
                                });
                            }
                            else
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "call", null, 0),
                                    $"Callee is not callable");
                            }
                        }

                        break;

                    case VMOpcode.Get:

                        {
                            if (VMInstruction.ReadString(CurrentCall.chunk, out var name, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "get", null, 0),
                                    $"Failed to read data");
                            }

                            var nameToken = new Scanner.Token(Scanner.TokenType.Identifier, name, name, 0);

                            var v = Pop();

                            var source = v.value;

                            if (source == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "get", null, 0),
                                    $"Source is valid");
                            }

                            TypeInfo typeInfo = null;

                            if (source is ScriptedInstance instance)
                            {
                                var value = instance.Get(nameToken);

                                if (value != null)
                                {
                                    if (value.attributes.HasFlag(VariableAttributes.ReadOnly))
                                    {
                                        CurrentCall.chunk.environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                                    }

                                    if (value.value is ScriptedProperty property)
                                    {
                                        Push(new VariableValue()
                                        {
                                            attributes = VariableAttributes.Set,
                                            value = property.GetFunction.Bind(instance).Call(nameToken, new List<object>()),
                                        });

                                        return;
                                    }

                                    Push(new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = value.value,
                                    });

                                    return;
                                }

                                typeInfo = TypeSystem.TypeSystem.FindType(instance.ScriptedClass.name);
                            }
                            else if (source != null)
                            {
                                typeInfo = TypeSystem.TypeSystem.FindType(source.GetType());
                            }

                            if (typeInfo != null)
                            {
                                var callableFunc = typeInfo.FindCallable(name);

                                if (callableFunc != null)
                                {
                                    var result = callableFunc(CurrentCall.chunk.environment);

                                    if (result.ParameterCount == 0)
                                    {
                                        var outValue = result.Bind(source).Call(nameToken, new List<object>());

                                        if (outValue is ScriptedProperty property)
                                        {
                                            if (property.GetFunction == null)
                                            {
                                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "get", null, 0),
                                                    $"Property {name} is not readable");
                                            }

                                            Push(new VariableValue()
                                            {
                                                attributes = VariableAttributes.Set,
                                                value = property.GetFunction.Bind(source).Call(nameToken, new List<object>()),
                                            });
                                        }
                                        else
                                        {
                                            Push(new VariableValue()
                                            {
                                                attributes = VariableAttributes.Set,
                                                value = outValue,
                                            });
                                        }

                                        return;
                                    }
                                    else
                                    {
                                        Push(new VariableValue()
                                        {
                                            attributes = VariableAttributes.Set,
                                            value = result.Bind(source),
                                        });

                                        return;
                                    }
                                }
                            }

                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "get", null, 0),
                                $"Source is not a class");
                        }

                    case VMOpcode.Set:

                        {
                            var targetValue = Pop();
                            var sourceValue = Pop();

                            if (VMInstruction.ReadString(CurrentCall.chunk, out var name, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Set, "set", null, 0),
                                    "Failed to read chunk data for Set");
                            }

                            var n = new Scanner.Token(Scanner.TokenType.Identifier, name, name, 0);

                            var source = sourceValue.value;
                            var value = targetValue.value;

                            if (source is ScriptedInstance instance)
                            {
                                var target = instance.Get(n);

                                if (target == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "set", null, 0),
                                        $"Property {name} doesn't exist");
                                }

                                if (target.value is ScriptedProperty property)
                                {
                                    if (property.SetFunction == null)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "set", null, 0),
                                            $"Property {name} is read-only");
                                    }

                                    property.SetFunction.Bind(source).Call(n, new List<object>(), (env) =>
                                    {
                                        env.Set("value", new VariableValue()
                                        {
                                            attributes = VariableAttributes.ReadOnly | VariableAttributes.Set,
                                            value = value,
                                        });
                                    });

                                    Push(new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = null,
                                    });

                                    return;
                                }

                                instance.Set(n, value, CurrentCall.chunk.environment);

                                Push(new VariableValue()
                                {
                                    value = value
                                });

                                return;
                            }

                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "set", null, 0),
                                $"Source is not a class");
                        }

                    case VMOpcode.Class:
                        {
                            if (VMInstruction.ReadString(CurrentCall.chunk, out var name, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadBool(CurrentCall.chunk, out var hasSupertype, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadInt32(CurrentCall.chunk, out var propertyCount, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadInt32(CurrentCall.chunk, out var readOnlyPropertyCount, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadInt32(CurrentCall.chunk, out var methodCount, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0), "Failed to read class data");
                            }

                            string superClassName = null;

                            if (hasSupertype && VMInstruction.ReadString(CurrentCall.chunk, out superClassName, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0), "Failed to read class supertype data");
                            }

                            TypeInfo superclass = null;
                            VMScriptedClass superClassInstance = null;

                            globalEnvironment.Set(name, new VariableValue() { value = null });

                            var environment = new VariableEnvironment(globalEnvironment);

                            if (superClassName != null)
                            {
                                superclass = TypeSystem.TypeSystem.FindType(superClassName);

                                if (superclass.scriptedClass == null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0), $"{superClassName} is not a class");
                                }

                                superClassInstance = superclass.scriptedClass;
                            }

                            if (superClassName != null)
                            {
                                environment = new VariableEnvironment(environment);
                                environment.Set("super", new VariableValue()
                                {
                                    attributes = VariableAttributes.ReadOnly | VariableAttributes.Set,
                                    value = superclass
                                });
                            }

                            var properties = new Dictionary<string, VariableValue>();

                            for (var i = 0; i < propertyCount; i++)
                            {
                                ExecuteOne();

                                string type;

                                if (VMInstruction.DeserializeVar(CurrentCall.chunk, out var propertyName, out type, out var hasInitializer,
                                    out var hasGetter, out var hasSetter, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Failed to read property data for class {name}");
                                }

                                VariableValue initializer = hasInitializer ? Pop() : null;

                                var typeInfo = type != null ? TypeSystem.TypeSystem.FindType(type) : null;

                                if (typeInfo == null && type != null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Var, "var", null, 0),
                                        $"Invalid variable type {type} for {propertyName}");
                                }

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
                                    (typeInfo.scriptedClass != null && initializer != null &&
                                    !TypeSystem.TypeSystem.Convert(initializer.value, typeInfo, out outValue)))
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Invalid initialization for property {propertyName} in class {name}");
                                }

                                VMScriptedFunction getter = null;
                                VMScriptedFunction setter = null;

                                if (hasGetter)
                                {
                                    _ = VMInstruction.ReadChunk(CurrentCall.chunk, ref CurrentCall.IP);

                                    if (VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    string returnType = null;

                                    if (hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                                    }

                                    var arguments = new Dictionary<string, string>();

                                    for (var j = 0; j < argumentCount; j++)
                                    {
                                        if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                            VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                        {
                                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                        }

                                        arguments.Add(argumentName, argumentType);
                                    }

                                    getter = new VMScriptedFunction(this, methodName, CurrentCall.chunk.environment, arguments, returnType, functionChunk, false);
                                }

                                if (hasSetter)
                                {
                                    _ = VMInstruction.ReadChunk(CurrentCall.chunk, ref CurrentCall.IP);

                                    if (VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    string returnType = null;

                                    if (hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                                    }

                                    var arguments = new Dictionary<string, string>();

                                    for (var j = 0; j < argumentCount; j++)
                                    {
                                        if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                            VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                        {
                                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                        }

                                        arguments.Add(argumentName, argumentType);
                                    }

                                    setter = new VMScriptedFunction(this, methodName, CurrentCall.chunk.environment, arguments, returnType, functionChunk, false);
                                }

                                if (hasGetter || hasSetter)
                                {
                                    outValue = new ScriptedProperty(getter, setter);
                                }

                                var attributes = VariableAttributes.None;

                                if (initializer != null)
                                {
                                    attributes |= VariableAttributes.Set;
                                }

                                var v = new VariableValue()
                                {
                                    attributes = attributes,
                                    typeInfo = typeInfo,
                                    value = outValue,
                                };

                                environment.Set(propertyName, v);

                                properties.Add(propertyName, v);
                            }

                            for (var i = 0; i < readOnlyPropertyCount; i++)
                            {
                                ExecuteOne();

                                string type;

                                if (VMInstruction.DeserializeVar(CurrentCall.chunk, out var propertyName, out type,
                                    out var hasInitializer, out var hasGetter, out var hasSetter, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Failed to read property data for class {name}");
                                }

                                VariableValue initializer = hasInitializer ? Pop() : null;

                                var typeInfo = type != null ? TypeSystem.TypeSystem.FindType(type) : null;

                                if (typeInfo == null && type != null)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Var, "var", null, 0),
                                        $"Invalid variable type {type} for {propertyName}");
                                }

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
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Invalid initialization for property {propertyName} in class {name}");
                                }

                                VMScriptedFunction getter = null;
                                VMScriptedFunction setter = null;

                                if (hasGetter)
                                {
                                    _ = VMInstruction.ReadChunk(CurrentCall.chunk, ref CurrentCall.IP);

                                    if (VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    string returnType = null;

                                    if (hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                                    }

                                    var arguments = new Dictionary<string, string>();

                                    for (var j = 0; j < argumentCount; j++)
                                    {
                                        if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                            VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                        {
                                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                        }

                                        arguments.Add(argumentName, argumentType);
                                    }

                                    getter = new VMScriptedFunction(this, methodName, CurrentCall.chunk.environment, arguments, returnType, functionChunk, false);
                                }

                                if (hasSetter)
                                {
                                    _ = VMInstruction.ReadChunk(CurrentCall.chunk, ref CurrentCall.IP);

                                    if (VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    string returnType = null;

                                    if (hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                                    }

                                    var arguments = new Dictionary<string, string>();

                                    for (var j = 0; j < argumentCount; j++)
                                    {
                                        if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                            VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                        {
                                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                        }

                                        arguments.Add(argumentName, argumentType);
                                    }

                                    setter = new VMScriptedFunction(this, methodName, CurrentCall.chunk.environment, arguments, returnType, functionChunk, false);
                                }

                                if (hasGetter || hasSetter)
                                {
                                    outValue = new ScriptedProperty(getter, setter);
                                }

                                var attributes = VariableAttributes.ReadOnly;

                                if (initializer != null)
                                {
                                    attributes |= VariableAttributes.Set;
                                }

                                var v = new VariableValue()
                                {
                                    attributes = attributes,
                                    typeInfo = typeInfo,
                                    value = outValue,
                                };

                                environment.Set(propertyName, v);

                                properties.Add(propertyName, v);
                            }

                            var functions = new Dictionary<string, VMScriptedFunction>();

                            for(var i = 0; i < methodCount; i++)
                            {
                                _ = VMInstruction.ReadChunk(CurrentCall.chunk, ref CurrentCall.IP);

                                if (VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                }

                                string returnType = null;

                                if (hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                }

                                if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                                }

                                var arguments = new Dictionary<string, string>();

                                for (var j = 0; j < argumentCount; j++)
                                {
                                    if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                        VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                    {
                                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                    }

                                    arguments.Add(argumentName, argumentType);
                                }

                                var scriptedFunction = new VMScriptedFunction(this, methodName, environment, arguments, returnType, functionChunk, methodName == "init");

                                functions.Add(methodName, scriptedFunction);
                            }

                            var scriptedClass = new VMScriptedClass(name, superClassInstance, functions, properties);

                            if (superclass != null)
                            {
                                environment = environment.parent;
                            }

                            environment = environment.parent;

                            environment.Assign(new Scanner.Token(Scanner.TokenType.Identifier, name, name, 0), scriptedClass);
                        }

                        break;

                    case VMOpcode.Function:

                        {
                            if (VMInstruction.ReadString(CurrentCall.chunk, out var name, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadString(CurrentCall.chunk, out var functionChunkName, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadBool(CurrentCall.chunk, out var hasReturnType, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadInt32(CurrentCall.chunk, out var argumentCount, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                            }

                            string returnType = null;

                            if(hasReturnType && VMInstruction.ReadString(CurrentCall.chunk, out returnType, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                            }

                            if (chunks.TryGetValue(functionChunkName, out var functionChunk) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to find function code");
                            }

                            var arguments = new Dictionary<string, string>();

                            for (var i = 0; i < argumentCount; i++)
                            {
                                if (VMInstruction.ReadString(CurrentCall.chunk, out var argumentName, ref CurrentCall.IP) == false ||
                                    VMInstruction.ReadString(CurrentCall.chunk, out var argumentType, ref CurrentCall.IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Func, "func", null, 0), "Failed to read function data");
                                }

                                arguments.Add(argumentName, argumentType);
                            }

                            var scriptedFunction = new VMScriptedFunction(this, name, globalEnvironment, arguments, returnType, functionChunk, false);

                            CurrentCall.chunk.environment.Set(name, new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = scriptedFunction
                            });
                        }

                        break;

                    case VMOpcode.NoOp:

                        return;

                    case VMOpcode.Super:

                        {
                            if(VMInstruction.ReadString(CurrentCall.chunk, out var methodName, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Super, "super", null, 0), "Failed to read super");
                            }

                            var superType = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.Super, "super", null, 0));
                            var thisInstance = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", null, 0));

                            if (superType == null || !(superType.value is TypeInfo superClass) ||
                                thisInstance == null || !(thisInstance.value is ScriptedInstance instance))
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Super, "super", null, 0),
                                    "Failed to handle super. We are likely not in a class method.");
                            }

                            var method = superClass.scriptedClass.FindMethod(methodName);

                            if(method == null)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Super, "super", null, 0),
                                    $"Method {methodName} not found for class {superType.typeInfo.name}");
                            }

                            Push(new VariableValue()
                            {
                                attributes = VariableAttributes.Set | VariableAttributes.ReadOnly,
                                value = method.Bind(thisInstance.value)
                            });
                        }

                        break;

                    case VMOpcode.This:

                        {
                            var thisInstance = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", null, 0));

                            if(thisInstance == null || !(thisInstance.value is ScriptedInstance instance))
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Super, "super", null, 0), "Failed to handle this. We are likely not in a class method.");
                            }

                            Push(thisInstance);
                        }

                        break;

                    default:

                        throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Comma, "", null, 0), $"Invalid opcode {instruction}");
                }
            }
            catch (RuntimeErrorException e)
            {
                Console.WriteLine($"Runtime Exception: {e.ToString()}");

                throw e;
            }
            catch(ReturnException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.ToString()}");

                throw e;
            }
        }

        internal static string Stringify(object o)
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

        internal void Bind(VMScriptedFunction function, VariableEnvironment environment, out VMChunk chunk)
        {
            chunk = new VMChunk(environment)
            {
                name = function.Chunk.name + $"_BIND_{++bindCounter}",
                code = function.Chunk.code,
                constants = function.Chunk.constants,
            };

            chunks.Add(chunk.name, chunk);
        }
    }
}
