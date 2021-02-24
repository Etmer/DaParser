﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DaScript
{
    public enum TokenType
    {
        ID, // => alphanumerical
        CONDITION, // => if

        EQUALS, // => =
        PLUS, // => +
        MINUS, // => -
        MUL, // => *
        DIV, // => /
        SEMI, // => ;

        L_PAREN, // => (
        R_PAREN, // => )

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
                    string s = Convert.ToString(value);
                    value = Regex.Match(s, @"(\')(.?*)(\')").Value;
                    break;
            }


            Value = value;
        }

        public T GetValue<T>() 
        {
            return (T)Value;
        }
    }
}