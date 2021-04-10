using System;
using System.Collections.Generic;
using System.Text;

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

    public class SymbolTable
    {
        private Dictionary<string, ISymbol> symbols = new Dictionary<string, ISymbol>();
        public SymbolTable() { DefineBuildInTypes(); }

        public void Define(ISymbol symbol) 
        {
            symbols.Add(symbol.Name, symbol);
        }

        public ISymbol LookUp(string name) 
        {
            return symbols[name];
        }

        private void DefineBuildInTypes() 
        {
            Define(new IntegerSymbol("Integer"));
            Define(new DoubleSymbol("Double"));
            Define(new StringSymbol("String"));
            Define(new BooleanSymbol("Boolean"));
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
}
