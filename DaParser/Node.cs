using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Node
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
    public class ConditionNode : Node
    {
        public ConditionNode(Token token, Node value, Node left, Node right) : base(token)
        {
            children.Add(value);
            children.Add(left);
            children.Add(right);
        }
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
    public class VariableDeclarationNode : Node
    {
        public Node Variable { get; private set; }
        public VariableDeclarationNode(Token token, Node declarationPart) : base(token)
        {
            Variable = declarationPart.children[0];
            children.Add(declarationPart);
        }
    }

    public class BlockNode : CompundStatementNode
    {
        public BlockNode(Token token, Node variable) : base(token)
        {
            children.Add(variable);
        }

        public override object GetValue()
        {
            return children[0].GetValue();
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
