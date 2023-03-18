using CodingFoxLang.Compiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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

        public StackFrame CurrentCall => callStack.LastOrDefault();

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
                            catch (Exception e)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   e.Message);
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
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   e.Message);
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
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   e.Message);
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
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   e.Message);
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
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "lessEqual", null, 0),
                                   e.Message);
                            }
                        }

                        break;

                    case VMOpcode.Greater:

                        {
                            var right = Pop();
                            var left = Pop();

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

                            Push(new VariableValue()
                            {
                                attributes = VariableAttributes.Set,
                                value = Compiler.IsEqual(left, right),
                            });
                        }

                        break;

                    case VMOpcode.NotEqual:

                        {
                            var right = Pop();
                            var left = Pop();

                            Push(new VariableValue()
                            {
                                attributes = VariableAttributes.Set,
                                value = Compiler.IsEqual(left, right) == false,
                            });
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
                            if (VMInstruction.ReadString(CurrentCall.chunk, out var name, ref CurrentCall.IP) == false ||
                                VMInstruction.ReadBool(CurrentCall.chunk, out var hasType, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "var", null, 0),
                                    $"Failed to read data");
                            }

                            string typeString = null;

                            if (hasType && VMInstruction.ReadString(CurrentCall.chunk, out typeString, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Var, "var", null, 0),
                                    $"Failed to read data");
                            }

                            if (VMInstruction.ReadBool(CurrentCall.chunk, out var hasInitializer, ref CurrentCall.IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Var, "var", null, 0),
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

                            if(CurrentCall.chunk.environment.Exists("this"))
                            {
                                var This = CurrentCall.chunk.environment.Get(new Scanner.Token(Scanner.TokenType.This, "this", "this", 0));

                                if (This != null && This.value is ScriptedInstance instance && instance.Exists(variableName))
                                {
                                    variable = instance.Get(token);
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

                                if (VMInstruction.DeserializeVar(CurrentCall.chunk, out var propertyName, out type, out var hasInitializer, ref CurrentCall.IP) == false)
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

                                if (VMInstruction.DeserializeVar(CurrentCall.chunk, out var propertyName, out type, out var hasInitializer, ref CurrentCall.IP) == false)
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
