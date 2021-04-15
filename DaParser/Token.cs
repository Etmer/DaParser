using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DaScript
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
        PROGRAM, // => Program start
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

        L_PAREN, // => (
        R_PAREN, // => )
        L_BLOCK, // => [
        R_BLOCK, // => ]

        BOOLEAN, // => bool
        NUMBER,  // => numbers
        STRING, // => '...'

        TYPESPEC, // => Integer, string bool, double

        EOF
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

        public object GetValue() 
        {
            return Value;
        }
    }
}
