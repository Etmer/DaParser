using EventScript.Interfaces;
using EventScript.Literals;
using System.Collections.Generic;

namespace EventScript.Utils
{
    public class ExpressionFactory
    {
        public static DialogueExpression CreateDialogueExpression(DialogueTextExpression text, List<IDialogueMember> choiceList, Token token) 
        {
            DialogueExpression expr = CreateNode<DialogueExpression>(token);

            expr.AddChoiceExpression(choiceList);
            expr.SetTextExpression(text);

            return expr;
        }
        public static DialogueTextExpression CreateTextMemberExpression(IExpression textLiteral, IExpression nextLiteral, Token token)
        {
            DialogueTextExpression expr = CreateNode<DialogueTextExpression>(token);

            expr.SetText(textLiteral);
            expr.SetNext(nextLiteral);

            return expr;
        }
        public static DialogueChoiceExpression CreateChoiceMemberExpression(IExpression condition, IExpression textLiteral, IExpression nextLiteral, Token token)
        {
            DialogueChoiceExpression expr = CreateNode<DialogueChoiceExpression>(token);

            expr.SetCondition(condition);
            expr.SetText(textLiteral);
            expr.SetNext(nextLiteral);

            return expr;
        }
        public static BlockStatement CreateBlockStatement(List<IExpression> statements, Token token) 
        {
            BlockStatement result = CreateNode<BlockStatement>(token);

            foreach (IExpression statement in statements) 
                result.Append(statement);

            return result;
        }

        public static BinaryExpression CreateBinary(IExpression left, IExpression right, TokenType operatorType, Token token) 
        {
            BinaryExpression result = CreateNode<BinaryExpression>(token);
            result.SetLeft(left);
            result.SetRight(right);
            result.SetOperatorType(operatorType);

            return result;
        }

        public static UnaryExpression CreateUnary(IExpression expression, TokenType operatorType, Token token)
        {
            UnaryExpression result = CreateNode<UnaryExpression>(token);
            result.SetExpression(expression);
            result.SetOperatorType(operatorType);

            return result;
        }

        public static AssignStatement CreateAssignStatement(Variable variable, IExpression expression, Token token) 
        {
            AssignStatement result = CreateNode<AssignStatement>(token);
            result.SetVariable(variable);
            result.SetExpression(expression);

            return result;
        }

        public static ConditionalExpression CreateConditionalExpression(ConditionBlock conditionBlock, ConditionalExpression elseCondition, Token token) 
        {
            ConditionalExpression result = CreateNode<ConditionalExpression>(token);

            result.SetIfBlock(conditionBlock);
            result.SetElseBlock(elseCondition);
            return result;
        }

        public static ConditionBlock CreateConditionalBlock(int precedence, IExpression expression, BlockStatement blockStmt, Token token)
        {
            ConditionBlock result = CreateNode<ConditionBlock>(token);

            result.SetCondition(expression);
            result.SetBlockStatement(blockStmt);
            result.SetPrecedence(precedence);

            return result;
        }

        public static DialogueActorExpression CreateDialogueActorExpression(IExpression actor, Token token)
        {
            DialogueActorExpression expr = CreateNode<DialogueActorExpression>(token);
            expr.SetText(actor);

            return expr;
        }
        public static DialogueMoodExpression CreateDialogueMoodExpression(IExpression mood, Token token)
        {
            DialogueMoodExpression expr = CreateNode<DialogueMoodExpression>(token);
            expr.SetText(mood);

            return expr;
        }

        public static StringLiteral CreateStringLiteral(string value, Token token) 
        {
            StringLiteral lit = CreateLiteral<StringLiteral>(token, value);
            return lit;
        }

        public static NumberLiteral CreateNumberLiteral(string value, Token token) 
        {
            NumberLiteral lit = CreateLiteral<NumberLiteral>(token, value);
            return lit;
        }

        public static BooleanLiteral CreateBooleanLiteral(string value, Token token)
        {
            BooleanLiteral lit = CreateLiteral<BooleanLiteral>(token, value);
            return lit;
        }
        public static DialogueTerminatorExpression CreateDialogueTerminatorExpression(Token token)
        {
            DialogueTerminatorExpression expr = CreateNode<DialogueTerminatorExpression>(token);
            return expr;
        }

        public static EndBlockExpression CreateEndBlockExpression(Token token) 
        {
            EndBlockExpression expr = CreateNode<EndBlockExpression>(token);
            return expr;
        }

        private static T CreateLiteral<T>(Token token, object value) where T : Literal, new()
        {
            T node = new T();
            node.SetToken(token);
            node.SetValue(value);
            return node;
        }

        private static T CreateNode<T>(Token token) where T : NodeBase, new() 
        {
            T node = new T();
            node.SetToken(token);
            return node;
        } 
    }
}
