using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace CodingFoxLang.Compiler.TypeSystem
{
    class TypeSystem
    {
        private static Dictionary<string, TypeInfo> types = new Dictionary<string, TypeInfo>();

        public static void RegisterDefaultTypes()
        {
            void Register<T>(string name, T value, TypeInfo.ConvertCallback convert, TypeInfo.BinaryOperationCallback binaryOp)
            {
                RegisterType(new TypeInfo(name, typeof(T), null, () => value, convert, binaryOp));
            }

            (bool, object) ConvertNumeric<T>(object a) where T : IConvertible
            {
                try
                {
                    return (true, System.Convert.ChangeType(a, typeof(T)));
                }
                catch (Exception e)
                {
                    return (false, null);
                }
            }

            bool ConvertNumericInternal<T>(object a, out T value) where T : IConvertible
            {
                try
                {
                    value = (T)System.Convert.ChangeType(a, typeof(T));

                    return true;
                }
                catch (Exception e)
                {
                    value = default(T);

                    return false;
                }
            }

            Register("bool", false,
                (a) =>
                {
                    if (a is bool b)
                    {
                        return (true, b);
                    }

                    return (false, null);
                },
                (left, right, op) =>
                {
                    if(left is bool a)
                    {
                        if(op == TypeInfo.BinaryOperation.Add && right is string str)
                        {
                            return (true, left.ToString() + str);
                        }

                        if(right is bool b)
                        {
                            switch (op)
                            {
                                case TypeInfo.BinaryOperation.Equal:

                                    return (true, a == b);

                                case TypeInfo.BinaryOperation.Different:

                                    return (true, a != b);
                            }
                        }
                    }

                    return (false, null);
                });

            Register("char", (char)0,
                (a) =>
                {
                    return ConvertNumeric<char>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary && ConvertNumericInternal<char>(left, out var a))
                    {
                        return (true, -a);
                    }

                    char b;

                    if (ConvertNumericInternal(left, out a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("uint8", (byte)0,
                (a) =>
                {
                    return ConvertNumeric<byte>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary)
                    {
                        return (false, null);
                    }

                    byte b;

                    if (ConvertNumericInternal<byte>(left, out var a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("int8", (sbyte)0,
                (a) =>
                {
                    return ConvertNumeric<sbyte>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary && ConvertNumericInternal<sbyte>(left, out var a))
                    {
                        return (true, -a);
                    }

                    sbyte b;

                    if (ConvertNumericInternal(left, out a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("uint16", (ushort)0,
                (a) =>
                {
                    return ConvertNumeric<ushort>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary)
                    {
                        return (false, null);
                    }

                    ushort b;

                    if (ConvertNumericInternal<ushort>(left, out var a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("int16", (short)0,
                (a) =>
                {
                    return ConvertNumeric<short>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary && ConvertNumericInternal<short>(left, out var a))
                    {
                        return (true, -a);
                    }

                    short b;

                    if (ConvertNumericInternal(left, out a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("uint32", (uint)0,
                (a) =>
                {
                    return ConvertNumeric<uint>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary)
                    {
                        return (false, null);
                    }

                    uint b;

                    if (ConvertNumericInternal<uint>(left, out var a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("int32", 0,
                (a) =>
                {
                    return ConvertNumeric<int>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary && ConvertNumericInternal<int>(left, out var a))
                    {
                        return (true, -a);
                    }

                    int b;

                    if (ConvertNumericInternal(left, out a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("uint64", (ulong)0,
                (a) =>
                {
                    return ConvertNumeric<ulong>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary)
                    {
                        return (false, null);
                    }

                    ulong b;

                    if (ConvertNumericInternal<ulong>(left, out var a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("int64", (long)0,
                (a) =>
                {
                    return ConvertNumeric<long>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary && ConvertNumericInternal<long>(left, out var a))
                    {
                        return (true, -a);
                    }

                    long b = 0;

                    if (ConvertNumericInternal<long>(left, out a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("float", (float)0,
                (a) =>
                {
                    return ConvertNumeric<float>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary && ConvertNumericInternal<float>(left, out var a))
                    {
                        return (true, -a);
                    }

                    float b;

                    if (ConvertNumericInternal(left, out a) == false)
                    {
                        return (false, null);
                    }

                    if(op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if(ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("double", (double)0,
                (a) =>
                {
                    return ConvertNumeric<double>(a);
                },
                (left, right, op) =>
                {
                    if (op == TypeInfo.BinaryOperation.Unary && ConvertNumericInternal<double>(left, out var a))
                    {
                        return (true, -a);
                    }

                    double b;

                    if (ConvertNumericInternal(left, out a) == false)
                    {
                        return (false, null);
                    }

                    if (op == TypeInfo.BinaryOperation.Add && right is string str)
                    {
                        return (true, a.ToString() + str);
                    }
                    else if (ConvertNumericInternal(right, out b) == false)
                    {
                        return (false, null);
                    }

                    switch (op)
                    {
                        case TypeInfo.BinaryOperation.Add:

                            return (true, a + b);

                        case TypeInfo.BinaryOperation.Subtract:

                            return (true, a - b);

                        case TypeInfo.BinaryOperation.Divide:

                            return (true, a / b);

                        case TypeInfo.BinaryOperation.Multiply:

                            return (true, a * b);

                        case TypeInfo.BinaryOperation.Greater:

                            return (true, a > b);

                        case TypeInfo.BinaryOperation.GreaterEqual:

                            return (true, a >= b);

                        case TypeInfo.BinaryOperation.Less:

                            return (true, a < b);

                        case TypeInfo.BinaryOperation.LessEqual:

                            return (true, a <= b);

                        case TypeInfo.BinaryOperation.Equal:

                            return (true, a == b);

                        case TypeInfo.BinaryOperation.Different:

                            return (true, a != b);
                    }

                    return (false, null);
                });

            Register("string", "",
                (a) =>
                {
                    if (a is string stringValue)
                    {
                        return (true, stringValue);
                    }

                    return (false, null);
                },
                (left, right, op) =>
                {
                    if (left is string a)
                    {
                        var b = VirtualMachine.Stringify(right);

                        switch (op)
                        {
                            case TypeInfo.BinaryOperation.Add:

                                return (true, a + b);

                            case TypeInfo.BinaryOperation.Equal:

                                return (true, a == b);

                            case TypeInfo.BinaryOperation.Different:

                                return (true, a != b);
                        }
                    }

                    return (false, null);
                });

            var typeInfo = FindType("string");

            typeInfo.RegisterCallable("length", (environment) => new NativeCallable(environment, 0,
                (env, args) => {
                    return new ScriptedProperty(new NativeCallable(env, 0, (getEnv, args) =>
                    {
                        return ((string)getEnv.Get(new Token(TokenType.Identifier, "this", null, 0)).value)?.Length ?? 0;
                    }), null);
                }));
        }

        public static TypeInfo FindType(string name)
        {
            return types.TryGetValue(name, out var type) ? type : null;
        }

        public static TypeInfo FindType(Type type)
        {
            foreach (var pair in types)
            {
                if (pair.Value.type == type)
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public static void RegisterType(TypeInfo typeInfo)
        {
            if (types.ContainsKey(typeInfo.name))
            {
                throw new ParseError();
            }

            types.Add(typeInfo.name, typeInfo);
        }

        public static bool Convert(object value, TypeInfo typeInfo, out object outValue)
        {
            if (typeInfo.scriptedClass != null && value is ScriptedInstance instance)
            {
                var sourceClass = instance.ScriptedClass;

                while (sourceClass != null)
                {
                    if (sourceClass.name == typeInfo.scriptedClass.name)
                    {
                        outValue = value;

                        return true;
                    }

                    sourceClass = sourceClass.SuperClass;
                }

                outValue = null;

                return false;
            }

            if (typeInfo.convertCallback == null)
            {
                outValue = null;

                return false;
            }

            var result = typeInfo.convertCallback(value);

            if (result.Item1 == false)
            {
                outValue = null;

                return false;
            }

            outValue = result.Item2;

            return true;
        }
    }
}
