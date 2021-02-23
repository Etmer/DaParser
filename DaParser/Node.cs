using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Node
    {
        public Token Token { get; private set; }
        public Node Left = null;
        public Node Right = null;

        public Node(Token token) 
        {
            this.Token = token;
        }
    }
}
