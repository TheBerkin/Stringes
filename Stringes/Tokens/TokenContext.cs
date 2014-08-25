using System;
using System.Collections.Generic;
using System.Linq;

namespace Stringes.Tokens
{
    /// <summary>
    /// Represents a set of rules for creating tokens from a stringe.
    /// </summary>
    /// <typeparam name="T">The identifier type to use in tokens created from the context.</typeparam>
    public sealed class TokenContext<T> : IEnumerable<Tuple<string, T>>
    {
        private readonly HashSet<char> _punctuation;
        private List<Tuple<string, T>> _list;
        private bool _sorted;

        public TokenContext()
        {
            _punctuation = new HashSet<char>();
            _list = new List<Tuple<string, T>>(8);
            _sorted = false;
        }

        /// <summary>
        /// Adds a rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
        /// </summary>
        /// <param name="symbol">The symbol to test for.</param>
        /// <param name="value">The token identifier to associate with the symbol.</param>
        public void Add(string symbol, T value)
        {
            if (_sorted) throw new InvalidOperationException("Cannot add entries after context has been used.");
            if (String.IsNullOrEmpty(symbol)) throw new ArgumentException("Argument 'symbol' can neither be null nor empty.");
            if (_list.All(t => t.Item1 != symbol)) _list.Add(Tuple.Create(symbol, value));
            _punctuation.Add(symbol[0]);
        }

        internal bool HasPunctuation(char c)
        {
            return _punctuation.Contains(c);
        }

        public IEnumerator<Tuple<string, T>> GetEnumerator()
        {
            if (_sorted) return ((IEnumerable<Tuple<string, T>>)_list).GetEnumerator();
            _list = _list.OrderByDescending(t => t.Item2).ToList();
            _sorted = true;
            return ((IEnumerable<Tuple<string, T>>)_list).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}