using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler
{
    partial class Interpreter
    {
        public object VisitVariableStatement(VariableStatement statement)
        {
            if(environment.Exists(statement.name.lexeme, false))
            {
                throw new RuntimeErrorException(statement.name, $"Variable `{statement.name.lexeme}' already exists.");
            }

            var typeInfo = statement.type != null ? TypeSystem.TypeSystem.FindType(statement.type.lexeme) : null;

            object value = null;

            if (statement.initializer != null)
            {
                value = Evaluate(statement.initializer);
            }

            if(typeInfo == null && value != null)
            {
                if (value is ScriptedInstance scriptedInstance)
                {
                    typeInfo = TypeSystem.TypeSystem.FindType(scriptedInstance.ScriptedClass.name);
                }
                else
                {
                    typeInfo = TypeSystem.TypeSystem.FindType(value.GetType());
                }
            }

            object outValue = null;

            if(typeInfo == null || (typeInfo.type != null && ((statement.initializer != null && value == null) || (value != null && !TypeSystem.TypeSystem.Convert(value, typeInfo, out outValue)))) ||
                (typeInfo.scriptedClass != null && value != null && !TypeSystem.TypeSystem.Convert(value, typeInfo, out outValue)))
            {
                throw new RuntimeErrorException(statement.name, $"Invalid value for `{statement.name.lexeme}'.");
            }

            if(statement.getStatements != null)
            {
                outValue = new ScriptedProperty(new NativeCallable(environment, 0, (env, args) =>
                    {
                        try
                        {
                            //TODO
                            //interpeter.ExecuteBlock(statement.getStatements, env);
                        }
                        catch (RuntimeErrorException e)
                        {
                            throw e;
                        }
                        catch (ReturnException e)
                        {
                            return e.value;
                        }

                        throw new RuntimeErrorException(statement.name, $"Expected return from getter for `{statement.name.lexeme}'.");
                    }),
                    statement.setStatements != null ? new NativeCallable(environment, 0, (env, args) =>
                    {
                        try
                        {
                            //TODO
                            //interpeter.ExecuteBlock(statement.setStatements, env);
                        }
                        catch (RuntimeErrorException e)
                        {
                            throw e;
                        }
                        catch (ReturnException e)
                        {
                            return e.value;
                        }

                        return null;
                    }) : null);
            }

            var variableValue = new VariableValue()
            {
                typeInfo = typeInfo,
                value = outValue,
            };

            if (outValue != null)
            {
                variableValue.attributes = VariableAttributes.Set;
            }

            environment.Set(statement.name.lexeme, variableValue);

            return null;
        }
    }
}
