using System.Collections.Generic;

namespace DaScript
{
    public enum Category 
    {
        Function,
        Variable,
        Block,
        BuiltIn,
        None
    }

    public class SymbolTable : ErrorRaiser
    {
        public SymbolTable ParentTable { get; private set; }
        private Dictionary<string, ISymbol> symbols = new Dictionary<string, ISymbol>();
        public SymbolTable() { DefineBuildInTypes(); }
        public SymbolTable(SymbolTable parent) { ParentTable = parent; }

        public bool Define(ISymbol symbol) 
        {
            if (NotYetDeclared(symbol.Name))
            {
                symbols.Add(symbol.Name, symbol);
                return true;
            }
            return false;
        }

        public bool LookUp(string name, out ISymbol symbol) 
        {
            symbol = null;
            bool result = symbols.ContainsKey(name);

            if (result)
                symbol = symbols[name];
            else if(ParentTable != null)
                result = ParentTable.LookUp(name, out symbol);

            return result;
        }

        public void DefineDialogueSymbols()
        {
            Define(new ChoiceSymbol("Choice"));
            Define(new TextSymbol("Text"));
            Define(new DialogueSymbol());
        }

        /// <summary>
        /// Returns true if symbol has not been declared in this or the parents symbol table
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool NotYetDeclared(string name) 
        {
            if (ParentTable != null)
            {
                if (ParentTable.NotYetDeclared(name))
                    return !symbols.ContainsKey(name);
            }

            return !symbols.ContainsKey(name); ;
        }

        /// <summary>
        /// built in types are: string, double, int and bool
        /// </summary>
        private void DefineBuildInTypes() 
        {
            Define(new IntegerSymbol("int"));
            Define(new DoubleSymbol("double"));
            Define(new StringSymbol("string"));
            Define(new BooleanSymbol("bool"));
        } 
    }

    public interface ISymbol
    {
        string Name { get; }
        Category Category { get; }

    }

    public class Symbol : ISymbol
    {
        public string Name { get; private set; }
        public Category Category { get; private set; }

        public Symbol(string name, Category category) 
        {
            Name = name;
            Category = category;
        }
    }

    public class BuiltInSymbol : Symbol 
    {
        public BuiltInSymbol(string name) : base(name, Category.BuiltIn) { }
    }

    public class IntegerSymbol : BuiltInSymbol
    {
        public IntegerSymbol(string name) : base(name) { }
    }

    public class StringSymbol : BuiltInSymbol
    {
        public StringSymbol(string name) : base(name) { }
    }

    public class DoubleSymbol : BuiltInSymbol
    {
        public DoubleSymbol(string name) : base(name) { }
    }

    public class BooleanSymbol : BuiltInSymbol
    {
        public BooleanSymbol(string name) : base(name) { }
    }

    public class VariableSymbol : Symbol
    {
        public ISymbol Type { get; private set; }
        public VariableSymbol(string name, ISymbol type) : base(name, Category.Variable) 
        {
            Type = type;
        }
    }

    public class TextSymbol : BuiltInSymbol
    {
        public TextSymbol(string name) : base(name) { }
    }

    public class ChoiceSymbol : BuiltInSymbol
    {
        public ChoiceSymbol(string name) : base(name) { }
    }

    public class DialogueSymbol : BuiltInSymbol
    {
        public DialogueSymbol() : base("Dialogue") { }
    }
}
