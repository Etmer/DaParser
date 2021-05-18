
using EventScript.Interfaces;

namespace EventScript.Literals
{
    public class Literal : IExpression
    {
        public object value;

        public Literal(object value) 
        {
            this.value = value;
        }

        public object Accept(IVisitor visitor)
        {
            return value;
        }
    }

    public class Variable : IExpression
    {
        public string Name { get; private set; }
        public Variable(string name) { Name = name; }

        public object Accept(IVisitor visitor) { return visitor.Visit_Variable(this);  
    }
}
