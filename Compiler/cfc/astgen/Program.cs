using System;
using System.IO;

namespace astgen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} [file] [outdir]");

                Environment.Exit(64);
            }
            else
            {
                try
                {
                    var grammar = File.ReadAllText(args[0]);

                    ASTGen.GenerateAST(grammar, args[1]);
                }
                catch(System.Exception)
                {
                    Console.WriteLine($"Failed to load AST file");

                    Environment.Exit(65);

                    return;
                }
            }
        }
    }
}
