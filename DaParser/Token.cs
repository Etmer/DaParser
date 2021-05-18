using System;

namespace EventScript
{
    public enum TokenType
    {
        NONE, //=> none: shouldthrow error
        ID, // => alphanumerical

        //Keywords
        CONDITION, // => if
        THEN, // => then
        ELSE, // => else
        ELSEIF, // => elif
        FUNC, // => functions
        DIALOGUESCRIPT, // => Program start Dialogue
        QUESTCRIPT, // => Program start Quest
        EMPTY, // => emtpy node
        END, // => Program end

        CALL, // => functions

        ASSIGN, // => =
        EQUALS, // => ==
        PLUS, // => +
        MINUS, // => -
        MUL, // => *
        DIV, // => /
        SEMI, // => ;
        COMMA, // => ,
        GREATER, // => >
        SMALLER, // => <
        SMALLEREQUALS, // => <=
        GREATEREQUALS, // => <=

        L_PAREN, // => (
        R_PAREN, // => )
        L_BLOCK, // => [
        R_BLOCK, // => ]

        BOOLEAN, // => bool
        NUMBER,  // => numbers
        STRING, // => '...'

        TYPESPEC, // => Integer, string bool, double

        EOF,

        //Dialogue specific types
        MEMBERDELIMITER_LEFT, // => {
        MEMBERDELIMITER_RIGHT, // => }
        TEXT_MEMBER,
        CHOICE_MEMBER,
        TRANSFER, // => arrow(=>)

    }
    public struct Token
    {
        public TokenType Type { get; private set; }
        public object Value { get; private set; }

        public int Line { get; private set; }
        public int Column { get; private set; }

        public static Token CreateEmpty() 
        {
            return new Token();
        }

        public void SetType(TokenType type) 
        {
            Type = type;
        }

        public void SetValue(object value) 
        {
            switch (Type)
            {
                case TokenType.NUMBER:
                    Value = Convert.ToDouble(value);
                    break;
                case TokenType.BOOLEAN:
                    Value = Convert.ToBoolean(value);
                    break;
                default:
                    Value = Convert.ToString(value);
                    break;
            }
        }

        public void SetPosition(int line, int column) 
        {
            Line = line;
            Column = column;
        }
    }
}
