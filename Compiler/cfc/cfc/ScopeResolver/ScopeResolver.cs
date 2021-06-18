using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingFoxLang.Compiler.ScopeResolver
{
    class ScopeResolver : IExpressionVisitor, IStatementVisitor
    {
        private enum FunctionType
        {
            None,
            Function,
            Initializer,
            Method
        }

        private enum ClassType
        {
            None,
            Class,
            Subclass
        }

        private List<Dictionary<string, bool>> scopes = new List<Dictionary<string, bool>>();
        private Interpreter interpreter;
        private FunctionType currentFunction = FunctionType.None;
        private ClassType currentClass = ClassType.None;

        public Action<int, string> Error;

        public ScopeResolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public void Resolve(List<IStatement> statements)
        {
            foreach(var statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(IStatement statement)
        {
            statement.Accept(this);
        }

        private void Resolve(IExpression expression)
        {
            expression.Accept(this);
        }

        private void BeginScope()
        {
            scopes.Add(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            scopes.RemoveAt(scopes.Count - 1);
        }

        private void Declare(Token name)
        {
            if(scopes.Count == 0)
            {
                return;
            }

            var scope = scopes[scopes.Count - 1];

            if(scope.ContainsKey(name.lexeme))
            {
                Error?.Invoke(name.line, $"Variable `{name.lexeme}' was already declared.");

                throw new ParseError();
            }
            else
            {
                scope.Add(name.lexeme, false);
            }
        }

        private void Define(Token name)
        {
            if(scopes.Count == 0)
            {
                return;
            }

            var scope = scopes[scopes.Count - 1];

            if (scope.ContainsKey(name.lexeme))
            {
                scope[name.lexeme] = true;
            }
        }

        private void ResolveLocal(IExpression expression, Token name)
        {
            for(int i = scopes.Count - 1; i >= 0; i--)
            {
                if(scopes[i].ContainsKey(name.lexeme))
                {
                    interpreter.Resolve(expression, scopes.Count - 1 - i);
                }
            }
        }

        private void ResolveFunction(FunctionStatement statement, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;

            BeginScope();

            foreach(var parameter in statement.parameters)
            {
                Declare(parameter.Item1);
                Define(parameter.Item1);
            }

            Resolve(statement.body);

            EndScope();

            currentFunction = enclosingFunction;
        }

        public object VisitSuperExpression(SuperExpression expression)
        {
            if(currentClass == ClassType.None)
            {
                Error?.Invoke(expression.keyword.line, "'super' is not available outside of a class.");

                throw new ParseError();
            }
            else if(currentClass != ClassType.Subclass)
            {
                Error?.Invoke(expression.keyword.line, "'super' cannot be used in a class with no superclass.");

                throw new ParseError();
            }

            ResolveLocal(expression, expression.keyword);

            return null;
        }

        public object VisitThisExpression(ThisExpression expression)
        {
            if(currentClass == ClassType.None)
            {
                Error?.Invoke(expression.keyword.line, "Invalid use of 'this' outside of a class.");

                throw new ParseError();
            }

            ResolveLocal(expression, expression.keyword);

            return null;
        }

        public object VisitSetExpression(SetExpression expression)
        {
            Resolve(expression.value);
            Resolve(expression.source);

            return null;
        }

        public object VisitGetExpression(GetExpression expression)
        {
            Resolve(expression.source);

            return null;
        }

        public object VisitClassStatement(ClassStatement statement)
        {
            var enclosingClass = currentClass;
            currentClass = ClassType.Class;

            Declare(statement.name);
            Define(statement.name);

            if(statement.superclass != null && statement.name.lexeme == statement.superclass.name.lexeme)
            {
                Error?.Invoke(statement.superclass.name.line, "Classes can't inherit from themselves.");

                throw new ParseError();
            }

            BeginScope();

            if(statement.superclass != null)
            {
                currentClass = ClassType.Subclass;

                Resolve(statement.superclass);
            }

            if(statement.superclass != null)
            {
                BeginScope();
                scopes[scopes.Count - 1].Add("super", true);
            }

            BeginScope();
            scopes[scopes.Count - 1].Add("this", true);
            scopes[scopes.Count - 1].Add(statement.name.lexeme, true);

            foreach(var property in statement.properties)
            {
                VisitVariableStatement(property);
            }

            foreach (var property in statement.readOnlyProperties)
            {
                VisitLetStatement(property);
            }

            foreach (var method in statement.methods)
            {
                var functionType = FunctionType.Method;

                if(method.name.lexeme == "init")
                {
                    functionType = FunctionType.Initializer;
                }

                ResolveFunction(method, functionType);
            }

            EndScope();

            if (statement.superclass != null)
            {
                EndScope();
            }

            EndScope();

            currentClass = enclosingClass;

            return null;
        }

        public object VisitAssignmentExpression(AssignmentExpression assignmentexpression)
        {
            Resolve(assignmentexpression.value);
            ResolveLocal(assignmentexpression, assignmentexpression.name);

            return null;
        }

        public object VisitBinaryExpression(BinaryExpression binaryexpression)
        {
            Resolve(binaryexpression.left);
            Resolve(binaryexpression.right);

            return null;
        }

        public object VisitBlockStatement(BlockStatement blockstatement)
        {
            BeginScope();
            Resolve(blockstatement.statements);
            EndScope();

            return null;
        }

        public object VisitCallExpression(CallExpression callexpression)
        {
            Resolve(callexpression.callee);

            foreach(var argument in callexpression.arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public object VisitExpressionStatement(ExpressionStatement expressionstatement)
        {
            Resolve(expressionstatement.expression);

            return null;
        }

        public object VisitForStatement(ForStatement forstatement)
        {
            return null;
        }

        public object VisitFunctionStatement(FunctionStatement functionstatement)
        {
            Declare(functionstatement.name);
            Define(functionstatement.name);

            ResolveFunction(functionstatement, FunctionType.Function);

            return null;
        }

        public object VisitGroupingExpression(GroupingExpression groupingexpression)
        {
            Resolve(groupingexpression.expression);

            return null;
        }

        public object VisitIfStatement(IfStatement ifstatement)
        {
            Resolve(ifstatement.condition);
            Resolve(ifstatement.thenBranch);

            if(ifstatement.elseBranch != null)
            {
                Resolve(ifstatement.elseBranch);
            }

            return null;
        }

        public object VisitLiteralExpression(LiteralExpression literalexpression)
        {
            return null;
        }

        public object VisitLogicalExpression(LogicalExpression logicalexpression)
        {
            Resolve(logicalexpression.left);
            Resolve(logicalexpression.right);

            return null;
        }

        public object VisitPrintStatement(PrintStatement printstatement)
        {
            Resolve(printstatement.expression);

            return null;
        }

        public object VisitReturnStatement(ReturnStatement returnstatement)
        {
            if(currentFunction == FunctionType.None)
            {
                Error?.Invoke(returnstatement.keyword.line, "Can't return from top-level code.");
            }

            if(returnstatement.value != null)
            {
                if(currentFunction == FunctionType.Initializer)
                {
                    Error?.Invoke(returnstatement.keyword.line, "Cannot return a value from an initializer.");

                    throw new ParseError();
                }

                Resolve(returnstatement.value);
            }

            return null;
        }

        public object VisitUnaryExpression(UnaryExpression unaryexpression)
        {
            Resolve(unaryexpression.right);

            return null;
        }

        public object VisitVariableExpression(VariableExpression variableexpression)
        {
            if(scopes.Count > 0 && scopes.All(x => x.TryGetValue(variableexpression.name.lexeme, out _) == false))
            {
                Error?.Invoke(variableexpression.name.line, "Can't read local variable in its own initializer.");

                throw new ParseError();
            }

            ResolveLocal(variableexpression, variableexpression.name);

            return null;
        }

        public object VisitVariableStatement(VariableStatement variablestatement)
        {
            Declare(variablestatement.name);

            if(variablestatement.initializer != null)
            {
                Resolve(variablestatement.initializer);
            }

            Define(variablestatement.name);

            return null;
        }

        public object VisitLetStatement(LetStatement letStatement)
        {
            Declare(letStatement.name);

            if (letStatement.initializer != null)
            {
                Resolve(letStatement.initializer);
            }

            Define(letStatement.name);

            return null;
        }

        public object VisitWhileStatement(WhileStatement whilestatement)
        {
            Resolve(whilestatement.condition);
            Resolve(whilestatement.body);

            return null;
        }
    }
}
