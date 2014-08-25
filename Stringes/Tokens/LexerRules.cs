using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stringes.Tokens
{
    /// <summary>
    /// Represents a set of rules for creating tokens from a stringe.
    /// </summary>
    /// <typeparam name="T">The identifier type to use in tokens created from the context.</typeparam>
    public sealed class LexerRules<T> : IEnumerable<Tuple<string, T>>
    {
        private readonly HashSet<char> _punctuation;
        private List<Tuple<string, T>> _list;
        private readonly List<Tuple<Regex, RuleMatchValueGenerator<T>>> _regexes; 
        private bool _sorted;

        public LexerRules()
        {
            _punctuation = new HashSet<char>();
            _list = new List<Tuple<string, T>>(8);
            _regexes = new List<Tuple<Regex, RuleMatchValueGenerator<T>>>(8);
            _sorted = false;
        }

        /// <summary>
        /// Adds a constant rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
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

        /// <summary>
        /// Adds a regex rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
        /// </summary>
        /// <param name="regex">The regex to test for.</param>
        /// <param name="value">The token identifier to associate with the symbol.</param>
        public void Add(Regex regex, T value)
        {
            if (_sorted) throw new InvalidOperationException("Cannot add entries after context has been used.");
            if (regex == null) throw new ArgumentNullException("regex");
            if (_regexes.All(re => re.Item1 != regex)) _regexes.Add(Tuple.Create(regex, new RuleMatchValueGenerator<T>(value)));
        }

        /// <summary>
        /// Adds a regex rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
        /// </summary>
        /// <param name="regex">The regex to test for.</param>
        /// <param name="generator">A function that generates a token identifier from the match.</param>
        public void Add(Regex regex, Func<Match, T> generator)
        {
            if (_sorted) throw new InvalidOperationException("Cannot add entries after context has been used.");
            if (regex == null) throw new ArgumentNullException("regex");
            if (generator == null) throw new ArgumentNullException("generator");
            if (_regexes.All(re => re.Item1 != regex)) _regexes.Add(Tuple.Create(regex, new RuleMatchValueGenerator<T>(generator)));
        }

        internal bool HasPunctuation(char c)
        {
            return _punctuation.Contains(c);
        }

        internal List<Tuple<Regex, RuleMatchValueGenerator<T>>> RegexList
        {
            get { return _regexes; }
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

    internal class RuleMatchValueGenerator<T>
    {
        private readonly T _value;
        private readonly Func<Match, T> _func;

        public RuleMatchValueGenerator(T value)
        {
            _value = value;
            _func = null;
        }

        public RuleMatchValueGenerator(Func<Match, T> generator)
        {
            _func = generator;
        }

        public T GetValue(Match m)
        {
            return _func == null ? _value : _func(m);
        }
    }
}