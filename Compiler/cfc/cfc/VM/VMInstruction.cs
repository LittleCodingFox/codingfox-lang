using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CodingFoxLang.Compiler
{
    internal class VMInstruction
    {
        public static void WriteChunk(VMChunk chunk, byte item)
        {
            chunk.code.Add(item);
        }

        public static byte ReadChunk(VMChunk chunk, ref int IP)
        {
            return chunk.code[IP++];
        }

        public static void Write(VMChunk chunk, VMDataType type, byte[] data)
        {
            WriteChunk(chunk, (byte)type);

            var b = BitConverter.GetBytes(data.Length);

            WriteChunk(chunk, (byte)b.Length);

            foreach(var c in b)
            {
                WriteChunk(chunk, c);
            }

            foreach(var c in data)
            {
                chunk.code.Add(c);
            }
        }

        public static void Read(VMChunk chunk, out VMDataType type, out object result, ref int IP)
        {
            var t = ReadChunk(chunk, ref IP);

            type = (VMDataType)t;

            var dataLengthSize = ReadChunk(chunk, ref IP);

            var dataLengthBytes = new byte[dataLengthSize];

            for(var i = 0; i < dataLengthSize; i++)
            {
                dataLengthBytes[i] = ReadChunk(chunk, ref IP);
            }

            var length = BitConverter.ToInt32(dataLengthBytes);

            var data = new byte[length];

            for(var i = 0; i < length; i++)
            {
                data[i] = ReadChunk(chunk, ref IP);
            }

            switch(type)
            {
                case VMDataType.Bool:

                    result = BitConverter.ToBoolean(data);

                    break;

                case VMDataType.Char:

                    result = BitConverter.ToChar(data);

                    break;

                case VMDataType.Uint8:

                    result = data[0];

                    break;

                case VMDataType.Int8:

                    result = (sbyte)data[0];

                    break;

                case VMDataType.Uint16:

                    result = BitConverter.ToUInt16(data);

                    break;

                case VMDataType.Int16:

                    result = BitConverter.ToInt16(data);

                    break;

                case VMDataType.Uint32:

                    result = BitConverter.ToUInt32(data);

                    break;

                case VMDataType.Int32:

                    result = BitConverter.ToInt32(data);

                    break;

                case VMDataType.Uint64:

                    result = BitConverter.ToUInt64(data);

                    break;

                case VMDataType.Int64:

                    result = BitConverter.ToInt64(data);

                    break;

                case VMDataType.Float:

                    result = BitConverter.ToSingle(data);

                    break;

                case VMDataType.Double:

                    result = BitConverter.ToDouble(data);

                    break;

                case VMDataType.String:

                    result = Encoding.UTF8.GetString(data);

                    break;

                default:

                    result = null;

                    return;
            }
        }

        public static void WriteBool(VMChunk chunk, bool value)
        {
            Write(chunk, VMDataType.Bool, BitConverter.GetBytes(value));
        }

        public static bool ReadBool(VMChunk chunk, out bool value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if(dataType != VMDataType.Bool || result == null)
            {
                value = default;

                return false;
            }

            value = (bool)result;

            return true;
        }

        public static void WriteChar(VMChunk chunk, char value)
        {
            Write(chunk, VMDataType.Char, BitConverter.GetBytes(value));
        }

        public static bool ReadChar(VMChunk chunk, out char value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Char || result == null)
            {
                value = default;

                return false;
            }

            value = (char)result;

            return true;
        }

        public static void WriteUint8(VMChunk chunk, byte value)
        {
            Write(chunk, VMDataType.Uint8, new byte[] { value });
        }

        public static bool ReadUint8(VMChunk chunk, out byte value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Uint8 || result == null)
            {
                value = default;

                return false;
            }

            value = (byte)result;

            return true;
        }

        public static void WriteInt8(VMChunk chunk, sbyte value)
        {
            Write(chunk, VMDataType.Int8, BitConverter.GetBytes(value));
        }

        public static bool ReadInt8(VMChunk chunk, out sbyte value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Int8 || result == null)
            {
                value = default;

                return false;
            }

            value = (sbyte)result;

            return true;
        }

        public static void WriteUint16(VMChunk chunk, ushort value)
        {
            Write(chunk, VMDataType.Uint16, BitConverter.GetBytes(value));
        }

        public static bool ReadUint16(VMChunk chunk, out ushort value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Uint16 || result == null)
            {
                value = default;

                return false;
            }

            value = (ushort)result;

            return true;
        }

        public static void WriteInt16(VMChunk chunk, short value)
        {
            Write(chunk, VMDataType.Int16, BitConverter.GetBytes(value));
        }

        public static bool ReadInt16(VMChunk chunk, out short value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Int16 || result == null)
            {
                value = default;

                return false;
            }

            value = (short)result;

            return true;
        }

        public static void WriteUint32(VMChunk chunk, uint value)
        {
            Write(chunk, VMDataType.Uint32, BitConverter.GetBytes(value));
        }

        public static bool ReadUint32(VMChunk chunk, out uint value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Uint32 || result == null)
            {
                value = default;

                return false;
            }

            value = (uint)result;

            return true;
        }

        public static void WriteInt32(VMChunk chunk, int value)
        {
            Write(chunk, VMDataType.Int32, BitConverter.GetBytes(value));
        }

        public static bool ReadInt32(VMChunk chunk, out int value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Int32 || result == null)
            {
                value = default;

                return false;
            }

            value = (int)result;

            return true;
        }

        public static void WriteUint64(VMChunk chunk, ulong value)
        {
            Write(chunk, VMDataType.Uint64, BitConverter.GetBytes(value));
        }

        public static bool ReadUint64(VMChunk chunk, out ulong value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Uint64 || result == null)
            {
                value = default;

                return false;
            }

            value = (ulong)result;

            return true;
        }

        public static void WriteInt64(VMChunk chunk, long value)
        {
            Write(chunk, VMDataType.Int64, BitConverter.GetBytes(value));
        }

        public static bool ReadInt64(VMChunk chunk, out long value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Int64 || result == null)
            {
                value = default;

                return false;
            }

            value = (long)result;

            return true;
        }

        public static void WriteFloat(VMChunk chunk, float value)
        {
            Write(chunk, VMDataType.Float, BitConverter.GetBytes(value));
        }

        public static bool ReadFloat(VMChunk chunk, out float value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Float || result == null)
            {
                value = default;

                return false;
            }

            value = (float)result;

            return true;
        }

        public static void WriteDouble(VMChunk chunk, float value)
        {
            Write(chunk, VMDataType.Double, BitConverter.GetBytes(value));
        }

        public static bool ReadDouble(VMChunk chunk, out double value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.Double || result == null)
            {
                value = default;

                return false;
            }

            value = (double)result;

            return true;
        }

        public static void WriteString(VMChunk chunk, string str)
        {
            Write(chunk, VMDataType.String, Encoding.UTF8.GetBytes(str));
        }

        public static bool ReadString(VMChunk chunk, out string value, ref int IP)
        {
            Read(chunk, out var dataType, out var result, ref IP);

            if (dataType != VMDataType.String || result == null)
            {
                value = default;

                return false;
            }

            value = (string)result;

            return true;
        }

        public static int AddConstant(VMChunk chunk, VariableValue value)
        {
            chunk.constants.Add(value);

            return chunk.constants.Count - 1;
        }

        public static void Constant(VMChunk chunk, int constant)
        {
            WriteChunk(chunk, (byte)VMOpcode.Constant);
            WriteChunk(chunk, (byte)constant);
        }

        public static void Variable(VMChunk chunk, string name)
        {
            WriteChunk(chunk, (byte)VMOpcode.Variable);
            WriteString(chunk, name);
        }

        public static void Add(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Add);
        }

        public static void Subtract(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Subtract);
        }

        public static void Multiply(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Multiply);
        }

        public static void Divide(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Divide);
        }

        public static void Greater(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Greater);
        }

        public static void GreaterEqual(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.GreaterEqual);
        }

        public static void Less(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Less);
        }

        public static void LessEqual(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.LessEqual);
        }

        public static void Equal(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Equal);
        }

        public static void NotEqual(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.NotEqual);
        }

        public static void Assign(VMChunk chunk, string name)
        {
            WriteChunk(chunk, (byte)VMOpcode.Assign);
            WriteString(chunk, name);
        }

        public static void AssignAt(VMChunk chunk, string name, int distance)
        {
            WriteChunk(chunk, (byte)VMOpcode.Assign);
            WriteString(chunk, name);
            WriteInt32(chunk, distance);
        }

        public static void Negate(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Negate);
        }

        public static void Let(VMChunk chunk, string name, Token type, IExpression initializer)
        {
            WriteChunk(chunk, (byte)VMOpcode.Let);
            WriteString(chunk, name);

            WriteBool(chunk, type != null);

            if (type != null)
            {
                WriteString(chunk, type.lexeme);
            }

            WriteBool(chunk, initializer != null);
        }

        public static void Var(VMChunk chunk, string name, Token type, IExpression initializer)
        {
            WriteChunk(chunk, (byte)VMOpcode.Var);
            WriteString(chunk, name);

            WriteBool(chunk, type != null);

            if (type != null)
            {
                WriteString(chunk, type.lexeme);
            }

            WriteBool(chunk, initializer != null);
        }

        public static void Return(VMChunk chunk)
        {
            WriteChunk(chunk, (byte)VMOpcode.Return);
        }
    }
}
