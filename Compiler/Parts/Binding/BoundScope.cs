using System.Collections.Immutable;

namespace Compiler.Parts.Binding
{
    internal sealed class BoundScope
    {
        private readonly Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

        public BoundScope? Parent { get; }

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public bool TryDeclare(VariableSymbol variable)
        {
            if (variable == null)
                throw new ArgumentNullException(nameof(variable));

            if (_variables.ContainsKey(variable.Name))
                return false;

            _variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookup(string name, out VariableSymbol? variable)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (_variables.TryGetValue(name, out variable))
                return true;

            return Parent?.TryLookup(name, out variable) ?? false;
        }

        public bool TryLookupInCurrentScope(string name, out VariableSymbol? variable)
        {
            return _variables.TryGetValue(name, out variable);
        }

        public bool TryUpdate(VariableSymbol variable)
        {
            if (variable == null)
                throw new ArgumentNullException(nameof(variable));

            if (_variables.ContainsKey(variable.Name))
            {
                _variables[variable.Name] = variable;
                return true;
            }

            return false;
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return _variables.Values.ToImmutableArray();
        }
    }
}

