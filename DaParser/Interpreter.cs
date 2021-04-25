﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Interpreter : ErrorRaiser, IVisitor
    {
        protected Memory GlobalMemory = new Memory();
        private Node tree = null;
        private SymbolTable symbolTable;

        public Interpreter() { }

        public void SetTree(Node node)
        {
            tree = node;
        }
        public void SetSymbolTable(SymbolTable table)
        {
            symbolTable = table;
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
                    result = Visit_AssignNode(node);
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
                case TokenType.TYPESPEC:
                    result = Visit_VariableDeclarationNode(node);
                    break;
                case TokenType.TEXT_MEMBER:
                    result = Visit_DialogueNode(node);
                    break;;
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
            return ((ITableValue)GlobalMemory[name]).GetValue();
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

            FunctionValue funcValue = (FunctionValue)GlobalMemory[name];


            return funcValue.GetValue(arguments);
        }

        public void EnterBlockNode(string name)
        {
            BlockValue blockNode = GlobalMemory[name] as BlockValue;
            Visit_CompoundNode(blockNode.GetBlock());
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

            if (node.Token.Type != TokenType.L_BLOCK)
                GlobalMemory[name] = Visit(node.children[1]);
            else
            {
                GlobalMemory[name] = node;
            }

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
            throw new System.Exception();
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
                    return +(double)Visit(node.children[0]);
                case TokenType.MINUS:
                    return -(double)Visit(node.children[0]);
            }
            throw new System.Exception();
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

        private object Visit_VariableDeclarationNode(Node node)
        {
            return Visit(node.children[0]);
        }

        private object Visit_DialogueNode(Node node)
        {
            foreach(Node subNode in node.children)
                Visit_DialogueMember(subNode);

            return 1;
        }
        private object Visit_DialogueMember(Node node)
        {
            Token token = node.Token;

            switch (token.Type) 
            {
                case TokenType.STRING:
                    return Visit_TextNode(node);
                case TokenType.CHOICE_MEMBER:
                    return Visit_TextChoiceNode(node);
                case TokenType.TRANSFER:
                    return Visit_TransferNode(node);
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }


        private object Visit_TextNode(Node node)
        {
            Dialogue dialogue = GlobalMemory["Dialogue"] as Dialogue;

            string text = (string)node.GetValue();
            dialogue.SetText(text);

            return 1;

        }

        private object Visit_TextChoiceNode(Node node)
        {
            Dialogue dialogue = GlobalMemory["Dialogue"] as Dialogue;

            string text = (string)Visit(node.children[0]); 
            string next = (string)Visit_TransferNode(node.children[1]);

            dialogue.SetOption(text, next);

            return 1;
        }
        private object Visit_ChoiceBlock(Node node)
        {
            Dialogue dialogue = GlobalMemory["Dialogue"] as Dialogue;
            Token token = node.Token;

            switch (token.Type)
            {
                case TokenType.MEMBERDELIMITER_LEFT:
                    foreach (Node subNode in node.children)
                        Visit_TextChoiceNode(subNode);
                    break;
                case TokenType.STRING:
                    dialogue.SetDefaultExit((string)Visit(node));
                    break;
            }

            return 1;
        }

        private object Visit_TransferNode(Node node)
        {
            Node next = node.children[0];
            Token token = next.Token;

            return Visit_ChoiceBlock(next);
            return 1;
        }
    }
}
