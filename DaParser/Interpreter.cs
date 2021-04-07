using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Interpreter : IVisitor
    {
        protected Table GlobalMemory = new Table();
        private Node tree = null;
       
        public Interpreter(Node tree) 
        {
            this.tree = tree;
        }

        public void Interpret() 
        {
            Visit(tree);
        }

        public object Visit(Node node)
        {
            object result = null;

            switch (node.Token.Type)
            {
                case TokenType.PROGRAM:
                    result = Visit_MainNode(node);
                    break;
                case TokenType.MINUS:
                case TokenType.PLUS:
                    result = Visit_UnaryNode(node);
                    break;
                case TokenType.MUL:
                case TokenType.DIV:
                case TokenType.EQUALS:
                    result = Visit_BinOpNode(node);
                    break;
                case TokenType.NUMBER:
                    result = Visit_NumberNode(node);
                    break;
                case TokenType.L_BLOCK:
                    result = Visit_CompoundNode(node);
                    break;
                case TokenType.ID:
                    result = Visit_VariableNode(node);
                    break;
                case TokenType.ASSIGN:
                    result = Visit_AssignNode(node);
                    break;
                case TokenType.CONDITION:
                    result = Visit_ConditionNode(node);
                    break;
                case TokenType.STRING:
                    result = Visit_StringNode(node);
                    break;
                case TokenType.CALL:
                    result = Visit_FunctionCallNode(node);
                    break;
            }
            return result;
        }

        private object Visit_CompoundNode(Node node) 
        {
            CompundStatementNode c_node = node as CompundStatementNode;

            foreach (Node n in c_node.statementList) 
            {
                Visit(n);
            }
            return 1;
        }

        private object Visit_VariableNode(Node node)
        {
            string name = (string)node.GetValue();
            return GlobalMemory[name];
        }

        private object Visit_FunctionCallNode(Node node)
        {
            FunctionCallNode funcCall = node as FunctionCallNode;
            string name = funcCall.Callee;

            List<object> arguments = new List<object>();

            foreach (Node argument in funcCall.Arguments) 
            {
                object value = Visit(argument);
                arguments.Add(value);
            }

            return ((FunctionValue)GlobalMemory[name]).Call(arguments);
        }

        public void EnterBlockNode(string name)
        {
            EventValue symbol = GlobalMemory[name] as EventValue;
            Visit(symbol.GetValue() as BlockNode);
        }

        private object Visit_MainNode(Node node)
        {
            CompundStatementNode program = node as CompundStatementNode;
            foreach (Node statementNode in program.statementList) 
            {
                Visit(statementNode);
            }
            return program.statementList[0];
        }

        private object Visit_AssignNode(Node node)
        {
            string name = (string)node.children[0].GetValue();

            GlobalMemory[name] = Visit(node.children[1]);

            return 1;
        }

        private object Visit_BinOpNode(Node node)
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
            throw OnError(token.Type);
        }

        private object Visit_NumberNode(Node node) 
        {
            return (double)node.GetValue();
        }

        private object Visit_StringNode(Node node)
        {
            return (string)node.GetValue();
        }

        private object Visit_UnaryNode(Node node)
        {
            Token token = node.Token;
            switch (token.Type) 
            {
                case TokenType.PLUS:
                    return +(double)node.children[0].GetValue();
                case TokenType.MINUS:
                    return -(double)node.children[0].GetValue();
            }
            throw OnError(token.Type);
        }

        private object Visit_ConditionNode(Node node)
        {
            Token token = node.Token;

            switch (token.Type) 
            {
                case TokenType.CONDITION:
                case TokenType.ELSEIF:
                    bool value = (bool)Visit(node.children[0]);

                    if (value)
                        return Visit_CompoundNode(node.children[1]);
                    else 
                        return Visit_ConditionNode(node.children[2]);
                case TokenType.ELSE:
                    return Visit_CompoundNode(node.children[0]);
                case TokenType.END:
                    return 1;
                default:
                    throw new System.Exception();
            }
        }

        private System.Exception OnError(TokenType type) 
        {
            return new System.Exception($"Expected to handle Unary Node! Unexpected tokentype: {type}");
        }
    }
}
