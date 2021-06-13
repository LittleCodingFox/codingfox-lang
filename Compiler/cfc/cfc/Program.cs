using System;
using System.IO;

namespace CodingFoxLang.Compiler
{
    class Program
    {
        static bool HasError = false;

        static void Main(string[] args)
        {
            if(args.Length > 1)
            {
                Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} [file]");

                Environment.Exit(64);
            }
            else if(args.Length == 1)
            {
                ProcessFile(args[0]);
            }
            else
            {
                Prompt();
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

        private static void Prompt()
        {
            for(; ; )
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                if(line == null)
                {
                    break;
                }

                Process(line, "??");
            }
        }

        private static void Process(string source, string path)
        {
            HasError = false;

            var scanner = new Scanner.Scanner(source)
            {
                Error = ErrorCallback
            };

            scanner.ScanTokens();

            var parser = new Parser.Parser(scanner.Tokens)
            {
                Error = ErrorCallback
            };

            var expression = parser.Parse();

            if(HasError || expression == null)
            {
                return;
            }

            Console.WriteLine(new Utilities.ASTPrinter().Print(expression));
        }

        private static void ErrorCallback(int line, string message)
        {
            Console.WriteLine($"Error at line {line}: {message}");

            HasError = true;
        }
    }
}
