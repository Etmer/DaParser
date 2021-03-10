using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class Parser
    {
        private Token currentToken;
        private Lexer lexer = null;

        private Dictionary<string, List<TokenType>> expectedTokens = new Dictionary<string, List<TokenType>>()
        {
            { "TERM", new List<TokenType>(){ TokenType.MINUS, TokenType.PLUS} },
            { "FACTOR", new List<TokenType>(){ TokenType.MUL, TokenType.DIV, TokenType.EQUALS} },
        };

        public Parser(Lexer lexer) 
        {
            this.lexer = lexer;
        }

        public Node Parse() 
        {
            ConsumeEntryToken();
            return Consume_Program();
        }

        private Node Consume_Expression()
        {
            Node node = Consume_Term();

            while (expectedTokens["TERM"].Contains(currentToken.Type))
            {
                Token token = currentToken;

                if (currentToken.Type == TokenType.PLUS)
                {
                    Consume(TokenType.PLUS);
                }
                if (currentToken.Type == TokenType.MINUS)
                {
                    Consume(TokenType.MINUS);
                }

                node = new BinaryOpNode(token, node, Consume_Term());
            }
            return node;
        }

        private void Consume(params TokenType[] expectedTypes)
        {
            Token token = currentToken;

            foreach (TokenType type in expectedTypes)
            {
                if (token.Type == type)
                {
                    if (currentToken.Type != TokenType.EOF)
                    {
                        currentToken = lexer.GetNextToken();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Program terminated successfully.");
                        return;
                    }

                }
            }
            throw new System.Exception();
        }

        private bool ConsumeEntryToken() 
        {
            Token token = lexer.GetNextToken();
            currentToken = token;

            return true;
        }

        private Node Consume_Factor() 
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                    Consume(TokenType.MINUS, TokenType.PLUS);
                    Node value = Consume_Expression();
                    return new UnaryNode(token, value);
                case TokenType.NUMBER:
                    Consume(TokenType.NUMBER);
                    return new NumberNode(token);
                case TokenType.L_PAREN:
                    Consume(TokenType.L_PAREN);
                    Node expr = Consume_Expression();
                    Consume(TokenType.R_PAREN);
                    return expr;
                case TokenType.ID:
                    Node var = Consume_Variable();
                    return var;
                case TokenType.STRING:
                    Consume(TokenType.STRING);
                    Node stringValue = new StringNode(token);
                    return stringValue;
            }
            throw new System.Exception();
        }

        private Node Consume_Term()
        {
            Node node = Consume_Factor();

            Token token = currentToken;
            while (expectedTokens["FACTOR"].Contains(currentToken.Type))
            {
                switch (token.Type)
                {
                    case TokenType.MUL:
                        Consume(TokenType.MUL);
                        break;
                    case TokenType.DIV:
                        Consume(TokenType.DIV);
                        break;
                    case TokenType.EQUALS:
                        Consume(TokenType.EQUALS);
                        break;
                }
                node = new BinaryOpNode(token, node, Consume_Factor());
            }
            return node;
        }

        private Node Consume_Program()
        {
            Consume(TokenType.PROGRAM);
            Node result = Consume_Function();
            Consume(TokenType.END);
            Consume(TokenType.EOF);
            return result;
        }

        private Node Consume_Function() 
        {
            Token token = currentToken;

            Consume(TokenType.FUNC);
            List<Node> nodes = Consume_StatementList();
            FunctionNode function = new FunctionNode(token);

            foreach (Node node in nodes)
                function.Append(node);

            Consume(TokenType.END);
            return function;
        }

        private Node Consume_CompoundStatement()
        {
            Token token = currentToken;

            List<Node> nodes = Consume_StatementList();
            CompundStatementNode compund = new CompundStatementNode(token);

            foreach (Node node in nodes)
                compund.Append(node);

            return compund;
        }

        private List<Node> Consume_StatementList()
        {
            Node node = Consume_Statement();
            List<Node> nodes = new List<Node>() { node };

            bool procede = currentToken.Type != TokenType.END;

            while (procede)
            {
                switch (currentToken.Type)
                {
                    case TokenType.ELSEIF:
                    case TokenType.ELSE:
                    case TokenType.END:
                        procede = false;
                        break;
                    default:
                        nodes.Add(Consume_Statement());
                        break;
                }
            }
            return nodes;
        }

        private Node Consume_Statement()
        {
            Token token = currentToken;
            switch (token.Type) 
            {
                case TokenType.ID:
                    return Consume_AssignStatement();
                case TokenType.CONDITION:
                case TokenType.ELSEIF:
                    Consume(TokenType.CONDITION, TokenType.ELSEIF);
                    Node value = Consume_Expression();
                    Node left = Consume_Then();
                    Node right = Consume_Else();

                    Node node = new ConditionNode(token, value, left, right);
                    return node;
            }
            throw new System.Exception();
        }

        private Node Consume_Then() 
        {
            Token token = currentToken;
            Consume(TokenType.THEN);
            return Consume_CompoundStatement();
        }

        private Node Consume_Else()
        {
            Token token = currentToken;
            switch (token.Type) 
            {
                case TokenType.ELSEIF:
                    return Consume_Statement();
                case TokenType.ELSE:
                    Consume(TokenType.ELSE);
                    Node node = Consume_CompoundStatement();
                    Consume(TokenType.END);
                    return  node;
                case TokenType.END:
                    Consume(TokenType.END);
                    return new EmptyNode(token);
            }
            throw new System.Exception();
        }

        private Node Consume_Variable()
        {
            Token token = currentToken;
            Consume(TokenType.ID);

            return new VariableNode(token);
        }

        private Node Consume_AssignStatement()
        {
            Node left = Consume_Variable();
            Token token = currentToken;

            Consume(TokenType.ASSIGN); 
            Node assign_right = Consume_Expression();
            Consume(TokenType.SEMI);
            return new AssignmentNode(token, left, assign_right);
        }
    }
}
