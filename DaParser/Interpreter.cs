using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Interpreter : IVisitor
    {
        public Dictionary<string, object> Globals = new Dictionary<string, object>();
        private Node tree = null;
       
        public Interpreter(Node tree) 
        {
            this.tree = tree;
        }

        public void Interpret() 
        {
            object value = Visit(tree);
            int i = 0;
        }

        public object Visit(Node node)
        {
            object result = null;

            switch (node.Token.Type)
            {
                case TokenType.MINUS:
                case TokenType.PLUS:
                case TokenType.MUL:
                case TokenType.DIV:
                    result = Handle_BinOpNode(node);
                    break;
                case TokenType.NUMBER:
                    result = Handle_NumberNode(node);
                    break;
                case TokenType.FUNC:
                    result = Handle_CompoundNode(node);
                    break;
                case TokenType.ID:
                    result = Handle_VariableNode(node);
                    break;
                case TokenType.ASSIGN:
                    result = Handle_AssignNode(node);
                    break;
                case TokenType.CONDITION:
                    result = Handle_ConditionNode(node);
                    break;

            }
            return result;
        }

        private object Handle_CompoundNode(Node node) 
        {
            CompundStatementNode c_node = node as CompundStatementNode;

            foreach (Node n in c_node.statementList) 
            {
                Visit(n);
            }
            return 1;
        }
        private object Handle_VariableNode(Node node) 
        {
            string name = node.GetValue<string>();

            if (Globals.ContainsKey(name))
                return Globals[name];

            throw new System.Exception();
        }
        private object Handle_AssignNode(Node node)
        {
            string name = node.children[0].GetValue<string>();

            if (Globals.ContainsKey(name))
                Globals.Add(name, Visit(node.children[1]));
            else
                Globals[name] = Visit(node.children[1]);

            return 1;
        }
        private object Handle_BinOpNode(Node node)
        {
            Token token = node.Token;

            switch (token.Type)
            {
                case TokenType.PLUS:
                    return (double)Visit(node.children[0]) + (double)Visit(node.children[1]);
                case TokenType.MINUS:
                    return (double)Visit(node.children[0]) - (double)Visit(node.children[1]);
                case TokenType.MUL:
                    return (double)Visit(node.children[0]) * (double)Visit(node.children[1]);
                case TokenType.DIV:
                    return (double)Visit(node.children[0]) / (double)Visit(node.children[1]);
                case TokenType.EQUALS:
                    object lhs = Visit(node.children[0]);
                    object rhs = Visit(node.children[1]);

                    return lhs.Equals(rhs);
            }
            throw new Exception();
        }

        private object Handle_NumberNode(Node node) 
        {
            return node.GetValue<double>();
        }
        private object Handle_ConditionNode(Node node)
        {
            Token rhs = node.children[2].Token;

            bool value = (bool)Handle_BinOpNode(node.children[0]);

            if (value)
                return Handle_CompoundNode(node.children[1]);
            else
                switch (rhs.Type) 
                {
                    case TokenType.END:
                        return 0;
                    case TokenType.ELSE:
                        return Handle_CompoundNode(node.children[0]);
                }

            throw new System.Exception();
        }
    }
}
