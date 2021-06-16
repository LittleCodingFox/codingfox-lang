using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace astgen
{
    static class ASTGen
    {
        public static void GenerateAST(string grammar, string statementGrammar, string path)
        {
            var lines = grammar.Split("\n".ToCharArray())
                .Select(x => x.Trim())
                .ToArray();

            List<string> allClasses = new List<string>();

            for(var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if(line.Length == 0)
                {
                    continue;
                }

                var split = line.Split(":".ToCharArray());

                if (split.Length != 2)
                {
                    Console.WriteLine($"Error on line {i + 1}: Invalid syntax. Expected a single split `:'");

                    continue;
                }

                var className = split[0].Trim();
                var fields = split[1].Trim();

                if (className.Length == 0 || fields.Length == 0)
                {
                    Console.WriteLine($"Error on line {i + 1}: Invalid syntax. Expected a valid class name and fields");

                    continue;
                }

                allClasses.Add(className);

                var outPath = Path.Combine(path, $"{className}.cs");

                try
                {
                    GenerateASTFile("IExpression", "IExpressionVisitor", className, fields, outPath);
                }
                catch(System.Exception)
                {
                    Console.WriteLine($"Error on line {i + 1}: Failed to generate AST data");

                    continue;
                }

                if(!File.Exists(outPath))
                {
                    Console.Write($"Error on line {i + 1}: Failed to create AST file");
                }
            }

            GenerateASTVisitorInterface("IExpressionVisitor", allClasses.ToArray(), Path.Combine(path, "IExpressionVisitor.cs"));

            allClasses.Clear();

            lines = statementGrammar.Split("\n".ToCharArray())
                .Select(x => x.Trim())
                .ToArray();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.Length == 0)
                {
                    continue;
                }

                var split = line.Split(":".ToCharArray());

                if (split.Length != 2)
                {
                    Console.WriteLine($"Error on line {i + 1}: Invalid syntax. Expected a single split `:'");

                    continue;
                }

                var className = split[0].Trim();
                var fields = split[1].Trim();

                if (className.Length == 0 || fields.Length == 0)
                {
                    Console.WriteLine($"Error on line {i + 1}: Invalid syntax. Expected a valid class name and fields");

                    continue;
                }

                allClasses.Add(className);

                var outPath = Path.Combine(path, $"{className}.cs");

                try
                {
                    GenerateASTFile("IStatement", "IStatementVisitor", className, fields, outPath);
                }
                catch (System.Exception)
                {
                    Console.WriteLine($"Error on line {i + 1}: Failed to generate AST data");

                    continue;
                }

                if (!File.Exists(outPath))
                {
                    Console.Write($"Error on line {i + 1}: Failed to create AST file");
                }
            }

            GenerateASTVisitorInterface("IStatementVisitor", allClasses.ToArray(), Path.Combine(path, "IStatementVisitor.cs"));
        }

        private static void GenerateASTFile(string baseClass, string visitorBaseClass, string className, string fields, string path)
        {
            var compileUnit = new CodeCompileUnit();
            var globalNamespace = new CodeNamespace();
            var codeNamespace = new CodeNamespace("CodingFoxLang.Compiler");

            compileUnit.Namespaces.Add(globalNamespace);
            compileUnit.Namespaces.Add(codeNamespace);

            codeNamespace.Imports.Add(new CodeNamespaceImport("CodingFoxLang.Compiler.Scanner"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            var classObject = new CodeTypeDeclaration(className)
            {
                IsClass = true,
            };

            classObject.TypeAttributes = (classObject.TypeAttributes & ~TypeAttributes.VisibilityMask) | TypeAttributes.NestedAssembly;

            classObject.BaseTypes.Add(baseClass);

            codeNamespace.Types.Add(classObject);

            var constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public
            };

            classObject.Members.Add(constructor);

            var fieldsList = fields.Split(",".ToCharArray()).Select(x => x.Trim());

            foreach (var field in fieldsList)
            {
                var fieldParts = field.Split(" ".ToCharArray());

                if(fieldParts.Length != 2)
                {
                    continue;
                }

                var type = fieldParts[0].Trim();
                var name = fieldParts[1].Trim();

                if(type == "object")
                {
                    classObject.Members.Add(new CodeMemberField(typeof(object), name)
                    {
                        Attributes = MemberAttributes.Public
                    });

                    constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), name));
                }
                else
                {
                    classObject.Members.Add(new CodeMemberField(type, name)
                    {
                        Attributes = MemberAttributes.Public
                    });

                    constructor.Parameters.Add(new CodeParameterDeclarationExpression(type, name));
                }

                constructor.Statements.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), name),
                    new CodeVariableReferenceExpression(name))
                );
            }

            var acceptFunction = new CodeMemberMethod()
            {
                Name = "Accept",
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = new CodeTypeReference(typeof(object)),
            };
            
            acceptFunction.Parameters.Add(new CodeParameterDeclarationExpression(visitorBaseClass, "visitor"));

            acceptFunction.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeVariableReferenceExpression("visitor"), $"Visit{className}", new CodeThisReferenceExpression())));

            classObject.Members.Add(acceptFunction);

            using (var stream = new StreamWriter(path, append: false))
            {
                using (var writer = new IndentedTextWriter(stream))
                {
                    var codeProvider = new CSharpCodeProvider();

                    codeProvider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
                }
            }
        }

        private static void GenerateASTVisitorInterface(string baseClass, string[] classNames, string path)
        {
            var compileUnit = new CodeCompileUnit();
            var globalNamespace = new CodeNamespace();
            var codeNamespace = new CodeNamespace("CodingFoxLang.Compiler");

            compileUnit.Namespaces.Add(globalNamespace);
            compileUnit.Namespaces.Add(codeNamespace);

            codeNamespace.Imports.Add(new CodeNamespaceImport("CodingFoxLang.Compiler.Scanner"));

            var interfaceObject = new CodeTypeDeclaration(baseClass)
            {
                IsInterface = true,
            };

            interfaceObject.TypeAttributes = (interfaceObject.TypeAttributes & ~TypeAttributes.VisibilityMask) | TypeAttributes.NestedAssembly;

            codeNamespace.Types.Add(interfaceObject);

            foreach(var className in classNames)
            {
                var functionDecl = new CodeMemberMethod();

                functionDecl.Name = $"Visit{className}";
                functionDecl.ReturnType = new CodeTypeReference(typeof(object));
                functionDecl.Parameters.Add(new CodeParameterDeclarationExpression(className, $"{className.ToLower()}"));

                interfaceObject.Members.Add(functionDecl);
            }

            using (var stream = new StreamWriter(path, append: false))
            {
                using (var writer = new IndentedTextWriter(stream))
                {
                    var codeProvider = new CSharpCodeProvider();

                    codeProvider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
                }
            }
        }
    }
}
