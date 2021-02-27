using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public abstract class Node
    {
        public Token Token { get; private set; }
        public List<Node> children { get; protected set; } = new List<Node>();

        public Node(Token token)
        {
            this.Token = token;
        }
        public Node(Token token, Node left, Node right)
        {
            this.Token = token;

            children.Add(left);
            children.Add(right);
        }
        public virtual object GetValue() { return Token.GetValue(); }
    }
    public class BinaryOpNode : Node
    {
        public BinaryOpNode(Token token, Node left, Node right) : base(token, left, right) { }

    }
    public class NumberNode : Node
    {
        public NumberNode(Token token) : base(token) { }

    }
    public class AssignmentNode : Node
    {
        public AssignmentNode(Token token, Node left, Node right) : base(token, left, right) { }

    }
    public class ConditionNode : Node
    {
        public ConditionNode(Token token, Node left, Node right, Node value) : base(token)
        {
            children.Add(value);
            children.Add(left);
            children.Add(right);
        }
    }
    public class ElseNode : Node
    {
        public ElseNode(Token token, Node body) : base(token)
        {
            children.Add(body);
        }
    }
    public class VariableNode : Node
    {
        public VariableNode(Token token) : base(token) { }

    }
    public class CompundStatementNode : Node
    {
        public List<Node> statementList { get; } = new List<Node>();
        public CompundStatementNode(Token token) : base(token) { }

        public void Append(Node node)
        {
            statementList.Add(node);
        }
    }
    public class FunctionNode : CompundStatementNode
    {
        public FunctionNode(Token token) : base(token) {  }
    }
    public class EmptyNode : Node
    {
        public EmptyNode(Token token) : base(token) { }
    }
    public class EndNode : Node
    {
        public EndNode(Token token) : base(token)
        {

        }
    }
}
