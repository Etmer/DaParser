using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public interface IVisitor
    {
        object Visit(Node node);
    }
}
