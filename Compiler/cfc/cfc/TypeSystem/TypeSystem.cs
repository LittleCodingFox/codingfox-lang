using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingFoxLang.Compiler.TypeSystem
{
    class TypeSystem
    {
        private static Dictionary<string, TypeInfo> types = new Dictionary<string, TypeInfo>();

        public static void RegisterDefaultTypes()
        {
            void Register<T>(string name, T value, Func<object, (bool, object)> convert)
            {
                RegisterType(new TypeInfo(name, typeof(T), null, () => value, convert));
            }

            (bool, object) ConvertNumeric<T>(object a) where T: IConvertible
            {
                if (a is char charValue)
                {
                    return (true, System.Convert.ChangeType(charValue, typeof(T)));
                }
                else if (a is byte byteValue)
                {
                    return (true, System.Convert.ChangeType(byteValue, typeof(T)));
                }
                else if (a is short shortValue)
                {
                    return (true, System.Convert.ChangeType(shortValue, typeof(T)));
                }
                else if (a is ushort ushortValue)
                {
                    return (true, System.Convert.ChangeType(ushortValue, typeof(T)));
                }
                else if (a is int intValue)
                {
                    return (true, System.Convert.ChangeType(intValue, typeof(T)));
                }
                else if (a is uint uintValue)
                {
                    return (true, System.Convert.ChangeType(uintValue, typeof(T)));
                }
                else if (a is long longValue)
                {
                    return (true, System.Convert.ChangeType(longValue, typeof(T)));
                }
                else if (a is ulong ulongValue)
                {
                    return (true, System.Convert.ChangeType(ulongValue, typeof(T)));
                }
                else if (a is float floatValue)
                {
                    return (true, System.Convert.ChangeType(floatValue, typeof(T)));
                }
                else if (a is double doubleValue)
                {
                    return (true, System.Convert.ChangeType(doubleValue, typeof(T)));
                }

                return (false, null);
            }

            Register("bool", (bool)false, (a) =>
            {
                if (a is bool b)
                {
                    return (true, b);
                }

                return (false, null);
            });

            Register("char", (char)0, (a) =>
            {
                return ConvertNumeric<char>(a);
            });

            Register("uint8", (byte)0, (a) =>
            {
                return ConvertNumeric<byte>(a);
            });

            Register("int8", (sbyte)0, (a) =>
            {
                return ConvertNumeric<sbyte>(a);
            });

            Register("uint16", (ushort)0, (a) =>
            {
                return ConvertNumeric<ushort>(a);
            });

            Register("int16", (short)0, (a) =>
            {
                return ConvertNumeric<short>(a);
            });

            Register("uint32", (uint)0, (a) =>
            {
                return ConvertNumeric<uint>(a);
            });

            Register("int32", 0, (a) =>
            {
                return ConvertNumeric<int>(a);
            });

            Register("uint64", (ulong)0, (a) =>
            {
                return ConvertNumeric<ulong>(a);
            });

            Register("int64", (long)0, (a) =>
            {
                return ConvertNumeric<long>(a);
            });

            Register("float", (float)0, (a) =>
            {
                return ConvertNumeric<float>(a);
            });

            Register("double", (double)0, (a) =>
            {
                return ConvertNumeric<double>(a);
            });

            Register("string", "", (a) =>
            {
                if (a is string stringValue)
                {
                    return (true, stringValue);
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
            foreach(var pair in types)
            {
                if(pair.Value.type == type)
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public static void RegisterType(TypeInfo typeInfo)
        {
            if(types.ContainsKey(typeInfo.name))
            {
                throw new ParseError();
            }

            types.Add(typeInfo.name, typeInfo);
        }

        public static bool Convert(object value, TypeInfo typeInfo, out object outValue)
        {
            if(typeInfo.scriptedClass != null && value is ScriptedInstance instance)
            {
                var sourceClass = instance.ScriptedClass;

                while(sourceClass != null)
                {
                    if(sourceClass.name == typeInfo.scriptedClass.name)
                    {
                        outValue = value;

                        return true;
                    }

                    sourceClass = sourceClass.SuperClass;
                }

                outValue = null;

                return false;
            }

            if(typeInfo.convertCallback == null)
            {
                outValue = null;

                return false;
            }

            var result = typeInfo.convertCallback(value);

            if(result.Item1 == false)
            {
                outValue = null;

                return false;
            }

            outValue = result.Item2;

            return true;
        }
    }
}
