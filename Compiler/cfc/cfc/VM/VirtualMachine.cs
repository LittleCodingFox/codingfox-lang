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
                var result = ExecuteOne();

                if(result != InterpretResult.OK)
                {
                    return result;
                }
            }
        }

        private InterpretResult ExecuteOne()
        {
            if (IP >= activeChunk.code.Count)
            {
                return InterpretResult.Exit;
            }

            var instruction = ReadByte();

            try
            {
                switch ((VMOpcode)instruction)
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

                                return InterpretResult.OK;
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

                                return InterpretResult.OK;
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

                                return InterpretResult.OK;
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

                                return InterpretResult.OK;
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

                            if (VMInstruction.ReadString(activeChunk, out var name, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "assign", null, 0),
                                    $"Failed to read data");
                            }

                            activeChunk.environment.Assign(new Scanner.Token(Scanner.TokenType.Identifier, name, name, 0), value.value);
                        }

                        break;

                    case VMOpcode.Let:

                        {
                            if (VMInstruction.ReadString(activeChunk, out var name, ref IP) == false ||
                                VMInstruction.ReadBool(activeChunk, out var hasType, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "let", null, 0),
                                    $"Failed to read data");
                            }

                            string typeString = null;

                            if (hasType && VMInstruction.ReadString(activeChunk, out typeString, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "let", null, 0),
                                    $"Failed to read data");
                            }

                            if (VMInstruction.ReadBool(activeChunk, out var hasInitializer, ref IP) == false)
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

                            activeChunk.environment.Set(name, new VariableValue()
                            {
                                attributes = attributes,
                                typeInfo = typeInfo,
                                value = outValue,
                            });
                        }

                        break;

                    case VMOpcode.Var:

                        {
                            if (VMInstruction.ReadString(activeChunk, out var name, ref IP) == false ||
                                VMInstruction.ReadBool(activeChunk, out var hasType, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "var", null, 0),
                                    $"Failed to read data");
                            }

                            string typeString = null;

                            if (hasType && VMInstruction.ReadString(activeChunk, out typeString, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "var", null, 0),
                                    $"Failed to read data");
                            }

                            if (VMInstruction.ReadBool(activeChunk, out var hasInitializer, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "var", null, 0),
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
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "var", null, 0),
                                    $"Variable {name} has invalid initialization");
                            }

                            var attributes = VariableAttributes.None;

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
                            if (VMInstruction.ReadString(activeChunk, out var variableName, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "variable", null, 0),
                                    $"Failed to read data");
                            }

                            var variable = activeChunk.environment.Get(new Scanner.Token(Scanner.TokenType.Identifier, variableName, variableName, 0));

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
                            if (VMInstruction.ReadInt32(activeChunk, out var argumentCount, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "call", null, 0),
                                    $"Failed to read data");
                            }

                            var callee = Pop();

                            var arguments = new List<object>();

                            for (var i = 0; i < argumentCount; i++)
                            {
                                arguments.Add(Pop());
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
                            if (VMInstruction.ReadString(activeChunk, out var name, ref IP) == false)
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
                                        activeChunk.environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                                    }

                                    if (value.value is ScriptedProperty property)
                                    {
                                        Push(new VariableValue()
                                        {
                                            attributes = VariableAttributes.Set,
                                            value = property.GetFunction.Bind(instance).Call(nameToken, new List<object>()),
                                        });

                                        return InterpretResult.OK;
                                    }

                                    Push(new VariableValue()
                                    {
                                        attributes = VariableAttributes.Set,
                                        value = value.value,
                                    });

                                    return InterpretResult.OK;
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
                                    var result = callableFunc(activeChunk.environment);

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

                                        return InterpretResult.OK;
                                    }
                                    else
                                    {
                                        Push(new VariableValue()
                                        {
                                            attributes = VariableAttributes.Set,
                                            value = result.Bind(source),
                                        });

                                        return InterpretResult.OK;
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

                            if (VMInstruction.ReadString(activeChunk, out var name, ref IP) == false)
                            {
                                return InterpretResult.RuntimeError;
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

                                    return InterpretResult.OK;
                                }

                                instance.Set(n, value, activeChunk.environment);

                                Push(new VariableValue()
                                {
                                    value = value
                                });

                                return InterpretResult.OK;
                            }

                            throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "set", null, 0),
                                $"Source is not a class");
                        }

                    case VMOpcode.Class:
                        {
                            if (VMInstruction.ReadString(activeChunk, out var name, ref IP) == false ||
                                VMInstruction.ReadBool(activeChunk, out var hasSupertype, ref IP) == false ||
                                VMInstruction.ReadInt32(activeChunk, out var propertyCount, ref IP) == false ||
                                VMInstruction.ReadInt32(activeChunk, out var readOnlyPropertyCount, ref IP) == false ||
                                VMInstruction.ReadInt32(activeChunk, out var methodCount, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0), "Failed to read class data");
                            }

                            string superClassName = null;

                            if (hasSupertype && VMInstruction.ReadString(activeChunk, out superClassName, ref IP) == false)
                            {
                                throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0), "Failed to read class supertype data");
                            }

                            object superclass = null;
                            ScriptedClass superClassInstance = null;

                            activeChunk.environment.Set(name, new VariableValue() { value = null });

                            activeChunk.environment = new VariableEnvironment(activeChunk.environment);

                            if (superClassName != null)
                            {
                                superclass = TypeSystem.TypeSystem.FindType(superClassName);

                                if (!(superclass is ScriptedClass))
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0), $"{superClassName} is not a class");
                                }

                                superClassInstance = (ScriptedClass)superclass;
                            }

                            if (superClassName != null)
                            {
                                activeChunk.environment = new VariableEnvironment(activeChunk.environment);
                                activeChunk.environment.Set("super", new VariableValue()
                                {
                                    attributes = VariableAttributes.ReadOnly | VariableAttributes.Set,
                                    value = superclass
                                });
                            }

                            var properties = new Dictionary<string, VariableValue>();

                            for (var i = 0; i < propertyCount; i++)
                            {
                                if (ExecuteOne() != InterpretResult.OK)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Failed to read property data for class {name}");
                                }

                                string type;

                                if (VMInstruction.DeserializeVar(activeChunk, out var propertyName, out type, out var hasInitializer, ref IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Failed to read property data for class {name}");
                                }

                                var exists = activeChunk.environment.Exists(propertyName);

                                VariableValue initializer = hasInitializer ? Pop() : null;

                                var typeInfo = type != null ? TypeSystem.TypeSystem.FindType(type) : null;

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

                                activeChunk.environment.Set(propertyName, v);

                                properties.Add(propertyName, v);

                                if (!exists)
                                {
                                    activeChunk.environment.Remove(propertyName);
                                }
                            }

                            for (var i = 0; i < readOnlyPropertyCount; i++)
                            {
                                if (ExecuteOne() != InterpretResult.OK)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Failed to read property data for class {name}");
                                }

                                string type;

                                if (VMInstruction.DeserializeVar(activeChunk, out var propertyName, out type, out var hasInitializer, ref IP) == false)
                                {
                                    throw new RuntimeErrorException(new Scanner.Token(Scanner.TokenType.Class, "class", null, 0),
                                        $"Failed to read property data for class {name}");
                                }

                                var exists = activeChunk.environment.Exists(propertyName);

                                VariableValue initializer = hasInitializer ? Pop() : null;

                                var typeInfo = type != null ? TypeSystem.TypeSystem.FindType(type) : null;

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

                                activeChunk.environment.Set(propertyName, v);

                                properties.Add(propertyName, v);

                                if (!exists)
                                {
                                    activeChunk.environment.Remove(propertyName);
                                }
                            }

                            var scriptedClass = new ScriptedClass(name, superClassInstance, new Dictionary<string, ScriptedFunction>(), properties);

                            if (superclass != null)
                            {
                                activeChunk.environment = activeChunk.environment.parent;
                            }

                            activeChunk.environment = activeChunk.environment.parent;

                            activeChunk.environment.Assign(new Scanner.Token(Scanner.TokenType.Identifier, name, name, 0), scriptedClass);
                        }

                        break;

                    default:

                        return InterpretResult.RuntimeError;
                }
            }
            catch (RuntimeErrorException e)
            {
                Console.WriteLine($"Runtime Exception: {e.message}");

                return InterpretResult.RuntimeError;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");

                return InterpretResult.RuntimeError;
            }

            return InterpretResult.OK;
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
