using EventScript.Interfaces;
using EventScript.Literals;
using EventScript.Utils;
using System.Collections.Generic;

namespace EventScript
{
    public class Parser : ErrorRaiser
    {
        private Token currentToken;
        private Lexer lexer = null;

        public Code Parse(Lexer lexer)
        {
            this.lexer = lexer;
            return Consume_Program();
        }

        public Code Consume_Program() 
        {
            currentToken = lexer.GetNextToken();
            BlockStatement block = new BlockStatement();
            //Consume(TokenType.DIALOGUESCRIPT);

            while (currentToken.Type != TokenType.EOF)
            {
                block.Append(Consume_ProgramBlock());
            }
            
            Consume(TokenType.EOF);

            Code code = new Code();
            code.SetBlockStatement(block);

            lexer.Reset();
            return code;
        }

        public BlockStatement Consume_BlockStatement() 
        { 
            BlockStatement block = new BlockStatement();
            block.AppendRange(Consume_StatementList());

            return block;
        }

        private Statement Consume_ProgramBlock()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.TYPESPEC:
                    return Consume_VariableDeclaration();
                
                case TokenType.L_BLOCK:
                    return Consume_BlockDeclaration();
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private DeclarationStatement Consume_BlockDeclaration()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.L_BLOCK:
                    Consume(TokenType.L_BLOCK);

                    BlockDeclarationExpression declStmt = new BlockDeclarationExpression();
                    string variableType = "Block";
                    string variableName = currentToken.Value.ToString();

                    StringLiteral lit = ExpressionFactory.CreateStringLiteral(variableName, currentToken);

                    declStmt.SetType(variableType);
                    declStmt.SetName(variableName);


                    Consume(TokenType.ID);
                    Consume(TokenType.R_BLOCK);

                    BlockStatement blockStmt = Consume_BlockStatement();
                    blockStmt.Append(Consume_EndBlock());

                    declStmt.SetExpression(blockStmt);

                    return declStmt;
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        public EndBlockExpression Consume_EndBlock()
        {
            Token token = currentToken;
            Consume(TokenType.END);

            return ExpressionFactory.CreateEndBlockExpression(token);
        }

        private List<IExpression> Consume_StatementList()
        {
            IExpression stmt = Consume_Statement();

            List<IExpression> statementList = new List<IExpression>() { stmt };

             while (currentToken.Type != TokenType.END)
            {
                statementList.Add(Consume_Statement());
            }

            return statementList;
        }

        private IExpression Consume_Statement()
        {
            Token token = currentToken;
            switch (token.Type)
            {
                case TokenType.ID:
                    IExpression stmt = Consume_ID();
                    Consume(TokenType.SEMI);
                    return stmt;

                case TokenType.CALL:
                    FunctionCallExpression call = Consume_FunctionCall();
                    Consume(TokenType.SEMI);
                    return call;

                case TokenType.CONDITION:
                    return Consume_ConditionalExpression();

                case TokenType.MEMBERDELIMITER_LEFT:
                    return Consume_DialogueExpression();
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private ConditionBlock Consume_ConditionBlock()
        {
            IExpression expr = Consume_Expression();
            BlockStatement blockStmt = new BlockStatement();

            Consume(TokenType.THEN);

            while (currentToken.Type != TokenType.ELSEIF && currentToken.Type != TokenType.ELSE) 
            {
                blockStmt.Append(Consume_Statement());
            }

            return ExpressionFactory.CreateConditionalBlock(0, expr, blockStmt, currentToken);
        }

        private ConditionalExpression Consume_ConditionalExpression()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.CONDITION:
                    Consume(TokenType.CONDITION);
                    return ExpressionFactory.CreateConditionalExpression(Consume_ConditionBlock(), Consume_ConditionalExpression(), currentToken);

                case TokenType.ELSEIF:
                    Consume(TokenType.ELSEIF);
                    ConditionBlock block = Consume_ConditionBlock();

                    if (currentToken.Type == TokenType.END) 
                    {
                        Consume(TokenType.END);
                        return ExpressionFactory.CreateConditionalExpression(block, null, currentToken);
                    }
                    else 
                        return ExpressionFactory.CreateConditionalExpression(block, Consume_ConditionalExpression(), currentToken);

                case TokenType.ELSE:
                    Consume(TokenType.ELSE); 
                    ConditionBlock elseBlock = ExpressionFactory.CreateConditionalBlock(2, new TRUE_Expression(), Consume_BlockStatement(), currentToken);
                    ConditionalExpression expr = ExpressionFactory.CreateConditionalExpression(elseBlock, null, currentToken);
                    Consume(TokenType.END);
                    return expr;
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private DeclarationStatement Consume_VariableDeclaration() 
        {

            DeclarationStatement declStmt = new DeclarationStatement();
            string variableType = currentToken.Value.ToString();
            Consume(TokenType.TYPESPEC);
            string variableName = currentToken.Value.ToString();

            declStmt.SetType(variableType);
            declStmt.SetName(variableName);

            Consume(TokenType.ID);
            Consume(TokenType.ASSIGN);
            IExpression expression = Consume_Expression();

            declStmt.SetExpression(expression);

            Consume(TokenType.SEMI);

            return declStmt;
        }

        private IExpression Consume_Expression()
        {
            IExpression result = Consume_Term();

            while (currentToken.Type == TokenType.PLUS || currentToken.Type == TokenType.MINUS)
            {
                Token token = currentToken;
                Consume(TokenType.PLUS, TokenType.MINUS);

                result = ExpressionFactory.CreateBinary(result, Consume_Term(), token.Type, currentToken);
            }

            return result;
        }

        private IExpression Consume_Term()
        {
            IExpression term = Consume_Factor();

            Token token = currentToken;
            while (IsOperator())
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
                    case TokenType.SMALLER:
                        Consume(TokenType.SMALLER);
                        break;
                    case TokenType.GREATER:
                        Consume(TokenType.GREATER);
                        break;
                    case TokenType.SMALLEREQUALS:
                        Consume(TokenType.SMALLEREQUALS);
                        break;
                    case TokenType.GREATEREQUALS:
                        Consume(TokenType.GREATEREQUALS);
                        break;
                }
                term = ExpressionFactory.CreateBinary(term,Consume_Factor(), token.Type, token);
            }
            return term;
        }

        private IExpression Consume_Factor()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                    Consume(TokenType.MINUS, TokenType.PLUS);
                    return ExpressionFactory.CreateUnary(Consume_Expression(), token.Type, token);

                case TokenType.NUMBER:
                    Consume(TokenType.NUMBER);
                    return ExpressionFactory.CreateNumberLiteral(token.Value.ToString(), token);

                case TokenType.L_PAREN:
                    Consume(TokenType.L_PAREN);
                    IExpression expr = Consume_Expression();
                    Consume(TokenType.R_PAREN);
                    return expr;

                case TokenType.ID:
                    return Consume_Variable();

                case TokenType.STRING:
                    Consume(TokenType.STRING);
                    return ExpressionFactory.CreateStringLiteral(token.Value.ToString(), token);

                case TokenType.CALL:
                    return Consume_FunctionCall();

            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private DialogueTerminatorExpression Consume_TerminationExpression() 
        {
            Token token = currentToken;
            Consume(TokenType.END);

            DialogueTerminatorExpression expr = ExpressionFactory.CreateDialogueTerminatorExpression(token);
            return expr;
        }

        private IExpression Consume_ID()
        {
            Token token = currentToken;
            Variable variable = Consume_Variable();

            switch (currentToken.Type)
            {
                case TokenType.ASSIGN:
                    Consume(TokenType.ASSIGN);
                    return ExpressionFactory.CreateAssignStatement(variable, Consume_Expression(), token);
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private FunctionCallExpression Consume_FunctionCall()
        {
            Token token = currentToken;

            FunctionCallExpression call = new FunctionCallExpression(token.Value.ToString());
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

        private Variable Consume_Variable()
        {
            Token token = currentToken;
            Consume(TokenType.ID);

            return new Variable(token.Value as string);
        }

        #region Dialogue

        private DialogueExpression Consume_DialogueExpression()
        {
            return ExpressionFactory.CreateDialogueExpression(Consume_TextMemberExpression(), Consume_DialogueMemberList(), currentToken);
        }

        private List<IExpression> Consume_DialogueMemberList()
        {
            List<IExpression> result = new List<IExpression>();

            while (currentToken.Type != TokenType.SEMI)
            {
               result.Add(Consume_DialogueMember());
            }

            Consume(TokenType.SEMI);
            return result;
        }

        private DialogueTextExpression Consume_TextMemberExpression()
        {
            Consume(TokenType.MEMBERDELIMITER_LEFT);
            Consume(TokenType.TEXT_MEMBER);
            Consume(TokenType.ASSIGN);

            IExpression text = Consume_Factor();

            Token token = currentToken;

            switch (token.Type) 
            {
                case TokenType.TRANSFER:
                    Consume(TokenType.TRANSFER);
                    IExpression next = null;

                    if (currentToken.Type == TokenType.END)
                        next = Consume_TerminationExpression();
                    else
                        next = Consume_Factor();

                    Consume(TokenType.MEMBERDELIMITER_RIGHT);
                    return ExpressionFactory.CreateTextMemberExpression(text, next, token);

                case TokenType.MEMBERDELIMITER_RIGHT:
                    Consume(TokenType.MEMBERDELIMITER_RIGHT);
                    return ExpressionFactory.CreateTextMemberExpression(text, null, token);
            }

            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, currentToken);
        }

        private DialogueChoiceExpression Consume_ChoiceExpression() 
        {
            Token token = currentToken;
            IExpression condition = new TRUE_Expression(); ;
            Consume(TokenType.CHOICE_MEMBER);
            Consume(TokenType.ASSIGN);

            IExpression text = Consume_Factor();

            Consume(TokenType.TRANSFER);

            IExpression next = null;
                 
            if (currentToken.Type == TokenType.END)
                next = Consume_TerminationExpression();
            else
                next = Consume_Factor();

            return ExpressionFactory.CreateChoiceMemberExpression(condition, text, next, token);
        }

        private IExpression Consume_DialogueMember()
        {
            Token token = currentToken;

            if (currentToken.Type == TokenType.L_PAREN)
            {
                IExpression condition = Consume_Expression();

                token = currentToken;
                Consume(TokenType.CONDITION);

                IExpression member = Consume_DialogueMember();

                return ExpressionFactory.CreateBinary(condition, member, token.Type, token);
            }

            IDialogueMember dialogueMember = null;
            Consume(TokenType.MEMBERDELIMITER_LEFT);

            switch (currentToken.Type)
            {
                case TokenType.ACTOR:
                    Consume(TokenType.ACTOR);
                    Consume(TokenType.ASSIGN);
                    dialogueMember = ExpressionFactory.CreateDialogueActorExpression(Consume_Factor(), token);
                    break;

                case TokenType.MOOD:
                    Consume(TokenType.MOOD);
                    Consume(TokenType.ASSIGN);
                    dialogueMember = ExpressionFactory.CreateDialogueMoodExpression(Consume_Factor(), token);
                    break;

                case TokenType.CHOICE_MEMBER:
                    dialogueMember = Consume_ChoiceExpression();
                    break;
            }

            Consume(TokenType.MEMBERDELIMITER_RIGHT);

            return dialogueMember;
        }

        #endregion

        #region HelperFunctions

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
                    return;
                }
            }

            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private bool IsOperator() 
        {
            return  currentToken.Type == TokenType.MUL ||
                    currentToken.Type == TokenType.DIV ||
                    currentToken.Type == TokenType.EQUALS ||
                    currentToken.Type == TokenType.GREATER ||
                    currentToken.Type == TokenType.GREATEREQUALS ||
                    currentToken.Type == TokenType.SMALLER ||
                    currentToken.Type == TokenType.SMALLEREQUALS;

        }

        #endregion
    }
}
