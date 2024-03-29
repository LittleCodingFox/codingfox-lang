﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CodingFoxLang.Compiler
{
    class Program
    {
        public static bool HasError = false;
        public static bool HasRuntimeError = false;
        private static Compiler compiler = new Compiler();

        static void Main(string[] args)
        {
            TypeSystem.TypeSystem.RegisterDefaultTypes();

            if(args.Length != 1)
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} [file]");

                Environment.Exit(64);
            }
            else if(args.Length == 1)
            {
                ProcessFile(args[0]);
            }

            if(HasError)
            {
                Environment.Exit(65);
            }
        }

        private static void ProcessFile(string path)
        {
            string fileData;

            try
            {
                fileData = File.ReadAllText(path);
            }
            catch(System.Exception)
            {
                Console.WriteLine($"Error: unable to read the file at `{path}'");

                return;
            }

            Process(fileData, path);
        }

        private static void Process(string source, string path)
        {
            HasError = false;
            HasRuntimeError = false;

            var scanner = new Scanner.Scanner(source)
            {
                Error = ErrorCallback
            };

            scanner.ScanTokens();

            var parser = new Parser.Parser(scanner.Tokens)
            {
                Error = ErrorCallback
            };

            List<IStatement> statements = null;

            try
            {
                statements = parser.Parse();
            }
            catch (ParseError)
            {
                return;
            }

            if(HasError)
            {
                return;
            }

            compiler.Error = SetErrorCallback;
            compiler.RuntimeError = RuntimeErrorCallback;

            if (HasError)
            {
                return;
            }

            compiler.Compile(statements);

            try
            {
                compiler.vm.Interpret();
            }
            catch(Exception e)
            {
                Console.WriteLine($"VM Exception: {e}");
            }
        }

        public static void RuntimeErrorCallback(RuntimeErrorException error)
        {
            Console.WriteLine($"{error.Message}\n[line {error.token.line}]");

            HasRuntimeError = true;
        }

        public static void SetErrorCallback()
        {
            HasError = true;
        }

        public static void ErrorCallback(int line, string message)
        {
            Console.WriteLine($"Error at line {line}: {message}");

            HasError = true;
        }
    }
}
