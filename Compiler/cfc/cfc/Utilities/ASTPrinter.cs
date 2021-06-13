using System;
using System.Collections.Generic;
using System.Text;

namespace CodingFoxLang.Compiler.Utilities
{
    class ASTPrinter : IVisitor
    {
        public string Print(IExpression expression)
        {
            return expression.Accept(this) as string;
        }

        public object VisitBinary(Binary binary)
        {
            return Parenthesize(binary.op.lexeme, binary.left, binary.right);
        }

        public object VisitGrouping(Grouping grouping)
        {
            return Parenthesize("group", grouping.expression);
        }

        public object VisitLiteral(Literal literal)
        {
            if(literal.value == null)
            {
                return "nil";
            }

            return literal.value.ToString();
        }

        public object VisitUnary(Unary unary)
        {
            return Parenthesize(unary.op.lexeme, unary.right);
        }

        private string Parenthesize(string name, params IExpression[] expressions)
        {
            var builder = new StringBuilder();

            builder.Append($"({name}");

            foreach(var expression in expressions)
            {
                builder.Append($" {expression.Accept(this)}");
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}
