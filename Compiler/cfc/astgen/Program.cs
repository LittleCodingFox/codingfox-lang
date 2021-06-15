using System;
using System.IO;

namespace astgen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} [grammarpath] [statementgrammarpath] [outdir]");

                Environment.Exit(64);
            }
            else
            {
                try
                {
                    var grammar = File.ReadAllText(args[0]);
                    var statementGrammar = File.ReadAllText(args[1]);

                    ASTGen.GenerateAST(grammar, statementGrammar, args[2]);
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
