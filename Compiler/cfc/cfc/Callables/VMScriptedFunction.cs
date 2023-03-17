using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;

namespace CodingFoxLang.Compiler
{
    class VMScriptedFunction : ICallable
    {
        private bool isInitializer;

        public VariableEnvironment Closure { get; private set; }

        public Dictionary<string, string> Parameters { get; private set; }

        public int ParameterCount => Parameters?.Count ?? 0;

        public string Name { get; private set; }

        public string ReturnType { get; private set; }

        public VMChunk Chunk { get; private set; }

        public VirtualMachine VM { get; private set; }

        public VMScriptedFunction(VirtualMachine owner, string name, VariableEnvironment environment, Dictionary<string, string> parameters, string returnType, VMChunk chunk, bool isInitializer)
        {
            foreach (var parameter in parameters)
            {
                var typeInfo = TypeSystem.TypeSystem.FindType(parameter.Value);

                if (typeInfo == null)
                {
                    throw new RuntimeErrorException(new Token(TokenType.Func, parameter.Key, parameter.Key, 0), $"Invalid parameter type for `${parameter.Key}'.");
                }
            }

            if (returnType != null)
            {
                var typeInfo = TypeSystem.TypeSystem.FindType(returnType);

                if (typeInfo == null)
                {
                    throw new RuntimeErrorException(new Token(TokenType.Return, returnType, returnType, 0), $"Invalid return type for `${name}'.");
                }
            }

            Closure = environment;
            Chunk = chunk;
            Parameters = parameters;
            ReturnType = returnType;
            Name = name;
            VM = owner;

            this.isInitializer = isInitializer;
        }

        public object Call(Token token, List<object> arguments, Action<VariableEnvironment> temporariesSetup = null)
        {
            var environment = new VariableEnvironment(Closure);

            var counter = 0;

            foreach(var pair in Parameters)
            {
                var typeInfo = TypeSystem.TypeSystem.FindType(pair.Value);

                if (!TypeSystem.TypeSystem.Convert(arguments[counter++], typeInfo, out var value))
                {
                    throw new RuntimeErrorException(token, $"Invalid value for parameter `{pair.Key}'.");
                }

                environment.Set(pair.Key, new VariableValue()
                {
                    attributes = VariableAttributes.Set,
                    typeInfo = typeInfo,
                    value = value
                });
            }

            temporariesSetup?.Invoke(environment);

            var previous = Chunk.environment;

            Chunk.environment = environment;

            var stackFrame = new VirtualMachine.StackFrame()
            {
                chunk = Chunk,
            };

            VM.callStack.Add(stackFrame);

            try
            {
                while(VM.CurrentCall.chunk == Chunk)
                {
                    if(VM.ExecuteOne() != InterpretResult.OK)
                    {
                        break;
                    }
                }
            }
            catch(RuntimeErrorException runtimeError)
            {
                Chunk.environment = previous;

                VM.callStack.Remove(stackFrame);

                return null;
            }
            catch (ReturnException returnValue)
            {
                Chunk.environment = previous;

                VM.callStack.Remove(stackFrame);

                if (isInitializer)
                {
                    return Closure.GetAt(0, "this");
                }

                var value = returnValue.value;

                if (ReturnType != null)
                {
                    var typeInfo = TypeSystem.TypeSystem.FindType(ReturnType);

                    if (typeInfo == null)
                    {
                        throw new RuntimeErrorException(token, $"Unexpected invalid return type for `{Name}'.");
                    }

                    if (!TypeSystem.TypeSystem.Convert(value, typeInfo, out var outValue))
                    {
                        throw new RuntimeErrorException(token, $"Invalid return value on call to `{Name}'.");
                    }

                    return outValue;
                }

                return returnValue.value;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception in {Name}: {e}");

                Chunk.environment = previous;

                VM.callStack.Remove(stackFrame);

                if (isInitializer)
                {
                    return Closure.GetAt(0, "this");
                }

                return null;
            }

            Chunk.environment = previous;

            VM.callStack.Remove(stackFrame);

            if (isInitializer)
            {
                return Closure.GetAt(0, "this");
            }

            return null;
        }

        public ICallable Bind(object instance)
        {
            var environment = new VariableEnvironment(Closure);

            environment.Set("this", new VariableValue()
            {
                attributes = VariableAttributes.Set,
                value = instance
            });

            return new VMScriptedFunction(VM, Name, environment, Parameters, ReturnType, Chunk, isInitializer);
        }
    }
}
