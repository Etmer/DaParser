using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DaScript
{
    public enum TokenType
    {
        ID, // => alphanumerical

        //Keywords
        CONDITION, // => if
        THEN, // => then
        ELSE, // => else
        ELSEIF, // => elif
        FUNC, // => functions
        PROGRAM, // => Program start
        EMPTY, // => emtpy node
        END, // => Program end

        ASSIGN, // => =
        EQUALS, // => ==
        PLUS, // => +
        MINUS, // => -
        MUL, // => *
        DIV, // => /
        SEMI, // => ;
        COMMA, // => ,

        L_PAREN, // => (
        R_PAREN, // => )
        L_BLOCK, // => [
        R_BLOCK, // => ]

        BOOLEAN, // => bool
        NUMBER,  // => numbers
        STRING, // => '...'

        EOF
    }
    public struct Token
    {
        public TokenType Type { get; private set; }
        public object Value { get; private set; }

        public static Token CreateEmpty() 
        {
            return new Token();
        }

        public void Set(TokenType type, object value)
        {
            Type = type;

            switch (type) 
            {
                case TokenType.NUMBER:
                    value = Convert.ToDouble(value);
                    break;
                case TokenType.BOOLEAN:
                    value = Convert.ToBoolean(value);
                    break;
                case TokenType.STRING:
                    value = Convert.ToString(value);
                    break;
            }


            Value = value;
        }

        public object GetValue() 
        {
            return Value;
        }
    }
}
