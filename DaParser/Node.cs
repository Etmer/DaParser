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
    public class StringNode : Node
    {
        public StringNode(Token token) : base(token) { }

    }
    public class AssignmentNode : Node
    {
        public AssignmentNode(Token token, Node left, Node right) : base(token, left, right) { }

    }
    public class ConditionNode : Node
    {
        public ConditionNode(Token token, Node value, Node left, Node right) : base(token)
        {
            children.Add(value);
            children.Add(left);
            children.Add(right);
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
    public class FunctionCallNode : Node
    {
        public string Callee;
        public List<Node> Arguments = new List<Node>();
        public FunctionCallNode(Token token, string callee) : base(token)
        {
            Callee = callee;
        }

        public void AddArgument(Node argument) 
        {
            Arguments.Add(argument);
        }
    }

    public class BlockNode : CompundStatementNode
    {
        public Node Variable { get; private set; }
        public BlockNode(Token token, Node variable) : base(token)
        {
            Variable = variable;
        }

        public override object GetValue()
        {
            return Variable.GetValue();
        }
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
    public class UnaryNode : Node
    {
        public TokenType op { get; private set; }
        public UnaryNode(Token token, Node value) : base(token)
        {
            op = token.Type;
            children.Add(value);
        }
    }


}
