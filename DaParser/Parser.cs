using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class Parser
    {
        private Token currentToken;
        private Token lastToken;
        private Lexer lexer = null;
        private bool process = true;

        private Dictionary<string, List<TokenType>> expectedTokens = new Dictionary<string, List<TokenType>>()
        {
            { "TERM", new List<TokenType>(){ TokenType.MINUS, TokenType.PLUS} },
            { "FACTOR", new List<TokenType>(){ TokenType.MUL, TokenType.DIV} },
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

        private bool Consume(params TokenType[] expectedTypes)
        {
            foreach (TokenType type in expectedTypes)
            {
                if (currentToken.Type == type)
                {
                    if (lexer.HasToken())
                    {
                        lastToken = currentToken;
                        currentToken = lexer.GetNextToken();
                        return true;
                    }
                    else
                    {
                        process = false;
                        return false;
                    }
                }
            }
            throw new System.Exception("Syntax Error");
        }

        private bool ConsumeEntryToken() 
        {
            Token token = lexer.GetNextToken();
            currentToken = token;
            lastToken = token;

            return true;
        }

        private Node Consume_Factor() 
        {
            Token token = currentToken;

            switch (token.Type)
            {
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
                }
                node = new BinaryOpNode(token, node, Consume_Factor());
            }
            return node;
        }

        private Node Consume_Program()
        {
            Consume(TokenType.PROGRAM);
            Node result = Consume_CompoundStatement();
            Consume(TokenType.END);
            return result;
        }

        private Node Consume_CompoundStatement()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.FUNC:
                case TokenType.THEN:
                case TokenType.ELSE:
                    Consume(TokenType.FUNC, TokenType.THEN, TokenType.ELSE);
                    List<Node> nodes = Consume_StatementList();
                    CompundStatementNode compund = new CompundStatementNode(token);

                    foreach (Node node in nodes)
                        compund.Append(node);

                    Consume(TokenType.END);
                    return compund;
                case TokenType.END:
                    return new EmptyNode(token);
            }
            throw new System.Exception();
        }

        private List<Node> Consume_StatementList()
        {
            Node node = Consume_Statement();
            List<Node> nodes = new List<Node>() { node };

            while (currentToken.Type == TokenType.SEMI)
            {
                Consume(TokenType.SEMI);
                if(currentToken.Type != TokenType.END)
                    nodes.Add(Consume_Statement());
            }

            return nodes;

        }
        private Node Consume_Statement()
        {
            Token token = currentToken;
            switch (token.Type) 
            {
                case TokenType.FUNC:
                    return Consume_CompoundStatement();
                case TokenType.ID:
                    return Consume_AssignStatement();
                case TokenType.CONDITION:
                    return Consume_IfStatement();
                case TokenType.END:
                    return new EndNode(token);
            }
            throw new System.Exception();
        }

        private Node Consume_IfStatement()
        {
            Token token = currentToken;
            Consume(TokenType.CONDITION);
            Consume(TokenType.L_PAREN);
            Node value = Consume_Statement();
            Consume(TokenType.R_PAREN);
            Node left = Consume_CompoundStatement(); 
            Node right = Consume_CompoundStatement();

            return new ConditionNode(token, left, right, value);
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
            switch (token.Type)
            {
                case TokenType.ASSIGN:
                    Consume(TokenType.ASSIGN); 
                    Node assign_right = Consume_Expression();
                    return new AssignmentNode(token, left, assign_right);
                case TokenType.EQUALS:
                    Consume(TokenType.EQUALS);
                    Node equal_right  = Consume_Expression();
                    return new BinaryOpNode(token, left, equal_right);
            }
            throw new Exception();
        }
    }
}
