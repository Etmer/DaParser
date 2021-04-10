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
            return Consume_Program();
        }

        //Consumes a Token until it reaches an EOF token
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

        //Program : DeclarationBlock -> ProgramBlock*
        private Node Consume_Program()
        {
            currentToken = lexer.GetNextToken();
            CompundStatementNode program = new CompundStatementNode(currentToken);
            Consume(TokenType.PROGRAM);

            while (currentToken.Type != TokenType.EOF)
            {
                program.Append(Consume_ProgramBlock());
            }
            Consume(TokenType.EOF);
            return program;
        }

        // Expr : Term(Plus|Minus -> Term)*
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

                node = new Node(token, node, Consume_Term());
            }
            return node;
        }

        // Factor : Number|LParen -> Expr -> RParen
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
                    return new Node(token);
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
                    Node stringValue = new Node(token);
                    return stringValue;
                case TokenType.CALL:
                    return Consume_FunctionCall();
            }
            throw new System.Exception();
        }

        //Term : Factor -> ((Mul|Div) -> Factor)*
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
                node = new Node(token, node, Consume_Factor());
            }
            return node;
        }

        //Var : Typespec -> Id -> Semi
        private Node Consume_VariableDeclaration()
        {
            Token token = currentToken;
            Consume(TokenType.TYPESPEC);
            Node varDecl = new VariableDeclarationNode(token, Consume_ID());
            Consume(TokenType.SEMI);
            return varDecl;
        }

        //FunctionBlock : LBlock -> BlockNode-> RBlock -> CompoundStatement -> End
        private Node Consume_BlockDeclaration()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.L_BLOCK:
                    Consume(TokenType.L_BLOCK);
                    BlockNode blockVar = new BlockNode(token, Consume_Variable());
                    Consume(TokenType.R_BLOCK);
                    List<Node> blockNodes = Consume_StatementList();

                    foreach (Node node in blockNodes)
                        blockVar.Append(node);

                    Consume(TokenType.END);

                    return blockVar;
            }
            throw new System.Exception();
        }

        //Block : Declaration|FunctionBlock
        private Node Consume_ProgramBlock()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.TYPESPEC:
                    return Consume_VariableDeclaration();
                case TokenType.L_BLOCK:
                    return Consume_BlockDeclaration();
            }
            throw new System.Exception();
        }

        //CompundStatement : Statement*
        private Node Consume_CompoundStatement()
        {
            Token token = currentToken;

            List<Node> nodes = Consume_StatementList();
            CompundStatementNode compund = new CompundStatementNode(token);

            foreach (Node node in nodes)
                compund.Append(node);

            return compund;
        }

        //StatementList : Statement*
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

        // Statement : VarDecl|Call|(Condition ->
        private Node Consume_Statement()
        {
            Token token = currentToken;
            switch (token.Type)
            {
                case TokenType.ID:
                    return Consume_ID();
                case TokenType.CALL:
                    Node call = Consume_FunctionCall();
                    Consume(TokenType.SEMI);
                    return call;
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

        private Node Consume_FunctionCall()
        {
            Token token = currentToken;

            FunctionCallNode call = new FunctionCallNode(token, token.Value.ToString());
            Consume(TokenType.CALL);
            Consume(TokenType.L_PAREN);

            while (currentToken.Type != TokenType.R_PAREN)
            {
                call.AddArgument(Consume_Term());
                if (currentToken.Type == TokenType.COMMA)
                    Consume(TokenType.COMMA);
            }

            Consume(TokenType.R_PAREN);
            return call;
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
                    return new Node(token);
            }
            throw new System.Exception();
        }

        private Node Consume_Variable()
        {
            Token token = currentToken;
            Consume(TokenType.ID);

            return new Node(token);
        }

        //Id : 
        private Node Consume_ID()
        {
            Token token = currentToken;
            Node variable = Consume_Variable();

            switch (currentToken.Type) 
            {
                case TokenType.ASSIGN:
                    token = currentToken;
                    Consume(TokenType.ASSIGN);
                    Node assign_right = Consume_Expression();
                    return new  Node(token, variable, assign_right);
            }
            throw new System.Exception();
        }
    }
}
