using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;

namespace CodingFoxLang.Compiler
{
    partial class Compiler : IExpressionVisitor, IStatementVisitor
    {
        public VirtualMachine vm = new VirtualMachine();

        public Action Error;
        public Action<RuntimeErrorException> RuntimeError;

        internal int functionCounter = 0;

        public Compiler()
        {
            RegisterCallable("clock", new NativeCallable(vm.globalEnvironment, 0, (env, args) => DateTimeOffset.Now.ToUnixTimeMilliseconds()));

            RegisterCallable("typeof", new NativeCallable(vm.globalEnvironment, 1, (env, args) =>
            {
                var value = args.Count > 0 ? args[0] : null;

                if(value == null)
                {
                    return "null";
                }

                if (value is ScriptedInstance scriptedInstance)
                {
                    return scriptedInstance.ScriptedClass.name;
                }
                else if (value is VMScriptedClass scriptedClass)
                {
                    return scriptedClass.name;
                }
                else if (value is VMScriptedFunction scriptedFunction)
                {
                    return $"{scriptedFunction.Name} (function)";
                }

                return value.GetType().Name;
            }));
        }

        public void RegisterCallable(string name, ICallable callable)
        {
            vm.globalEnvironment.Set(name, new VariableValue()
            {
                attributes = VariableAttributes.ReadOnly | VariableAttributes.Set,
                value = callable
            });
        }

        public bool Compile(List<IStatement> statements)
        {
            try
            {
                if (vm.activeChunk == null)
                {
                    vm.activeChunk = new VMChunk(vm.globalEnvironment);

                    vm.activeChunk.name = "main";

                    vm.chunks.Add(vm.activeChunk.name, vm.activeChunk);

                    vm.callStack.Add(new VirtualMachine.StackFrame()
                    {
                        chunk = vm.activeChunk,
                    });
                }

                foreach (var statement in statements)
                {
                    Compile(statement);
                }
            }
            catch (RuntimeErrorException error)
            {
                RuntimeError?.Invoke(error);

                return false;
            }
            catch (ParseError)
            {
                Error?.Invoke();

                return false;
            }

            return true;
        }

        public void Compile(IStatement statement)
        {
            statement.Accept(this);
        }

        public void Compile(List<IStatement> statements,
            VariableEnvironment environment)
        {
            var previous = vm.activeChunk.environment;

            try
            {
                vm.activeChunk.environment = environment;

                foreach (var statement in statements)
                {
                    Compile(statement);
                }
            }
            finally
            {
                vm.activeChunk.environment = previous;
            }
        }

        public void Resolve(string name, int depth)
        {
            if (!vm.activeChunk.locals.ContainsKey(name))
            {
                vm.activeChunk.locals.Add(name, depth);
            }
            else
            {
                vm.activeChunk.locals[name] = depth;
            }
        }

        private object LookupVariable(Token name)
        {
            if (vm.activeChunk.locals.TryGetValue(name.lexeme, out var distance))
            {
                var value = vm.activeChunk.environment.GetAt(distance, name.lexeme);

                if (value != null)
                {
                    if (value.attributes.HasFlag(VariableAttributes.ReadOnly))
                    {
                        vm.activeChunk.environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                    }

                    if (!value.attributes.HasFlag(VariableAttributes.Set))
                    {
                        throw new RuntimeErrorException(name, $"Variable has not been initialized");
                    }

                    return value.value;
                }
                else //TODO: Figure out an alternative for this. sometimes the locals will retain a reference to something that is in a farther away distance.
                {
                    var env = vm.activeChunk.environment;

                    while (env != null)
                    {
                        var result = env.Get(name);

                        if (result != null)
                        {
                            return result.value;
                        }

                        env = env.parent;
                    }
                }
            }
            else
            {
                var value = vm.activeChunk.environment.Get(name);

                if (value != null)
                {
                    if (value.attributes.HasFlag(VariableAttributes.ReadOnly))
                    {
                        vm.activeChunk.environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                    }

                    if (!value.attributes.HasFlag(VariableAttributes.Set))
                    {
                        throw new RuntimeErrorException(name, $"Variable has not been initialized");
                    }

                    return value.value;
                }
            }

            var globalValue = vm.globalEnvironment.Get(name);

            if (globalValue != null)
            {
                if (globalValue.attributes.HasFlag(VariableAttributes.ReadOnly))
                {
                    vm.activeChunk.environment.writeProtection = VariableEnvironment.WriteProtection.ReadOnly;
                }

                if (!globalValue.attributes.HasFlag(VariableAttributes.Set))
                {
                    throw new RuntimeErrorException(name, $"Variable has not been initialized");
                }

                return globalValue.value;
            }

            return null;
        }

        internal static bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private object Evaluate(IExpression expression)
        {
            return expression.Accept(this);
        }

        private void ValidateNumberType(Token op, params object[] operands)
        {
            TypeSystem.TypeInfo firstTypeInfo = null;

            foreach (var operand in operands)
            {
                if (operand == null)
                {
                    throw new RuntimeErrorException(op, "Operand must be a number.");
                }

                if (firstTypeInfo == null)
                {
                    if (operand is ScriptedInstance instance)
                    {
                        firstTypeInfo = instance.TypeInfo;
                    }
                    else
                    {
                        firstTypeInfo = TypeSystem.TypeSystem.FindType(operand.GetType());
                    }
                }

                if (firstTypeInfo == null || !TypeSystem.TypeSystem.Convert(operand, firstTypeInfo, out var _))
                {
                    throw new RuntimeErrorException(op, "Operand must be a number.");
                }
            }
        }

        private void HandleFunctionStatement(FunctionStatement statement)
        {
            var chunk = new VMChunk(vm.activeChunk.environment)
            {
                name = $"FUNCTION_{++functionCounter}_{statement.name.lexeme}",
            };

            var current = vm.activeChunk;

            vm.chunks.Add(chunk.name, chunk);
            vm.activeChunk = chunk;

            foreach (var s in statement.body)
            {
                s.Accept(this);
            }

            vm.activeChunk = current;

            var parameters = new Dictionary<string, string>();

            foreach (var p in statement.parameters)
            {
                parameters.Add(p.Item1.lexeme, p.Item2.lexeme);
            }

            VMInstruction.Function(vm.activeChunk, statement.name.lexeme, statement.returnType?.lexeme,
                parameters, chunk);
        }

        private void CreatePropertyMethod(string name, string returnType, List<IStatement> statements)
        {
            var chunk = new VMChunk(vm.activeChunk.environment)
            {
                name = $"FUNCTION_{++functionCounter}_{name}",
            };

            var current = vm.activeChunk;

            vm.chunks.Add(chunk.name, chunk);
            vm.activeChunk = chunk;

            foreach (var s in statements)
            {
                s.Accept(this);
            }

            vm.activeChunk = current;

            VMInstruction.Function(vm.activeChunk, name, returnType, new Dictionary<string, string>(), chunk);
        }
    }
}
