using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public abstract class Node
    {
        public Token Token { get; private set; }
        public Node Left { get; protected set; }
        public Node Right { get; protected set; }

        public Node(Token token)
        {
            this.Token = token;
        }
        public T GetValue<T>() { return Token.GetValue<T>(); }

    }
    public class BinaryOpNode : Node
    {
        public BinaryOpNode(Token token, Node left, Node right) : base(token)
        {
            Left = left;
            Right = right;
        }
    }
    public class NumberNode : Node
    {
        public NumberNode(Token token) : base(token) { }
    }
}
