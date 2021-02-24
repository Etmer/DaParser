using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Interpreter : IVisitor
    {
        private Node tree = null;

        public Interpreter(Node tree) 
        {
            this.tree = tree;
        }

        public void Interpret() 
        {
            object value = Visit(tree);
            Console.WriteLine(value);
        }

        public object Visit(Node node)
        {
            object result =  null;

            switch (node.Token.Type) 
            {
                case TokenType.MINUS:
                case TokenType.PLUS:
                case TokenType.MUL:
                case TokenType.DIV:
                    result = VisitBinOps(node);
                    break;
                case TokenType.NUMBER:
                    result = VisitNumber(node);
                    break;
            }
            return result;
        }

        private object VisitBinOps(Node node) 
        {
            switch (node.Token.Type) 
            {
                case TokenType.PLUS:
                    return (double)Visit(node.Left) + (double)Visit(node.Right);
                case TokenType.MINUS:
                    return (double)Visit(node.Left) - (double)Visit(node.Right);
                case TokenType.MUL:
                    return (double)Visit(node.Left) * (double)Visit(node.Right);
                case TokenType.DIV:
                    return (double)Visit(node.Left) / (double)Visit(node.Right);
            }
            throw new System.Exception();
        }
        private object VisitNumber(Node node)
        {
            return node.GetValue<double>();
        }
    }
}
