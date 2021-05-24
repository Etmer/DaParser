
using EventScript.Interfaces;

namespace EventScript.Literals
{
    public abstract class Literal : NodeBase, IExpression
    {
        public abstract string Type { get; protected set;}
        public object value { get; protected set; }

        public abstract object Accept(IVisitor visitor);
        public T GetValue<T>() { return (T)value; }

    }
    public class StringLiteral : Literal
    {
        public override string Type { get; protected set; } = "string"; 
        public StringLiteral(object value) { this.value = value.ToString(); }

        public override object Accept(IVisitor visitor)
        {
            return visitor.Visit_StringLiteral(this);
        }
    }

    public class NumberLiteral : Literal
    {
        public override string Type { get; protected set; } = "double";
        public NumberLiteral(object value) { this.value = double.Parse(value.ToString()); }

        public override object Accept(IVisitor visitor)
        {
            return visitor.Visit_NumberLiteral(this);
        }
    }

    public class BooleanLiteral : Literal
    {
        public override string Type { get; protected set; } = "bool";
        public BooleanLiteral(object value) { this.value = bool.Parse(value.ToString()); }

        public override object Accept(IVisitor visitor)
        {
            return visitor .Visit_BooleanLiteral(this);
        }
    }

    public class Variable : NodeBase, IExpression
    {
        public string Name { get; private set; }

        public Variable(string name)
        {
            Name = name; 
        }

        public object Accept(IVisitor visitor) { return visitor.Visit_Variable(this); }
    }
}
