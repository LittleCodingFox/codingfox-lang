using CodingFoxLang.Compiler.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Utilities
{
    class ScopeResolver : IExpressionVisitor, IStatementVisitor
    {
        private enum FunctionType
        {
            None,
            Function
        }

        private List<Dictionary<string, bool>> scopes = new List<Dictionary<string, bool>>();
        private Interpreter interpreter;
        private FunctionType currentFunction = FunctionType.None;

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
                Declare(parameter);
                Define(parameter);
            }

            Resolve(statement.body);

            EndScope();

            currentFunction = enclosingFunction;
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
            if(scopes.Count > 0 && scopes[scopes.Count - 1].TryGetValue(variableexpression.name.lexeme, out _) == false)
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

        public object VisitWhileStatement(WhileStatement whilestatement)
        {
            Resolve(whilestatement.condition);
            Resolve(whilestatement.body);

            return null;
        }
    }
}
