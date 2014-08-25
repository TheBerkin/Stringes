using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stringes.Tokens
{
    /// <summary>
    /// Represents a set of rules for creating tokens from a stringe.
    /// </summary>
    /// <typeparam name="T">The identifier type to use in tokens created from the context.</typeparam>
    public sealed class LexerRules<T> : IEnumerable
    {
        private const int DefaultPriority = 1;

        private readonly HashSet<char> _punctuation;
        private List<Tuple<string, T>> _listNormal;
        private List<Tuple<string, T>> _listHigh;
        private List<Tuple<Regex, RuleMatchValueGenerator<T>, int>> _regexes; 
        private bool _sorted;

        public LexerRules()
        {
            _punctuation = new HashSet<char>();
            _listNormal = new List<Tuple<string, T>>(8);
            _listHigh = new List<Tuple<string, T>>(8);
            _regexes = new List<Tuple<Regex, RuleMatchValueGenerator<T>, int>>(8);
            _sorted = false;
        }

        private bool Available(string symbol)
        {
            return _listNormal.All(t => t.Item1 != symbol) && _listHigh.All(t => t.Item1 != symbol);
        }

        /// <summary>
        /// Adds a constant rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
        /// </summary>
        /// <param name="symbol">The symbol to test for.</param>
        /// <param name="value">The token identifier to associate with the symbol.</param>
        /// <param name="priority">Determines whether the symbol should be tested before any regex rules.</param>
        public void Add(string symbol, T value, LexerConstantPriority priority = LexerConstantPriority.Normal)
        {
            if (_sorted) throw new InvalidOperationException("Cannot add entries after context has been used.");
            if (String.IsNullOrEmpty(symbol)) throw new ArgumentException("Argument 'symbol' can neither be null nor empty.");

            if (Available(symbol)) (priority == LexerConstantPriority.High ? _listHigh : _listNormal).Add(Tuple.Create(symbol, value));
            _punctuation.Add(symbol[0]);
        }

        /// <summary>
        /// Adds a constant rule to the context that affects all symbols in the specified array. This will throw an InvalidOperationException if called after the context is used to create tokens.
        /// </summary>
        /// <param name="symbols">The symbols to test for.</param>
        /// <param name="value">The token identifier to associate with the symbols.</param>
        /// <param name="priority">Determines whether the symbol should be tested before any regex rules.</param>
        public void Add(string[] symbols, T value, LexerConstantPriority priority = LexerConstantPriority.Normal)
        {
            if (_sorted) throw new InvalidOperationException("Cannot add entries after context has been used.");
            if (symbols == null) throw new ArgumentNullException("symbols");
            if (symbols.Length == 0) throw new ArgumentException("Tried to use an empty symbol array.");
            foreach (var s in symbols)
            {
                if (String.IsNullOrEmpty(s)) throw new ArgumentException("One or more symbols in the provided array were empty or null.");
                if (Available(s)) (priority == LexerConstantPriority.High ? _listHigh : _listNormal).Add(Tuple.Create(s, value));
                _punctuation.Add(s[0]);
            }
        }

        /// <summary>
        /// Adds a regex rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
        /// </summary>
        /// <param name="regex">The regex to test for.</param>
        /// <param name="value">The token identifier to associate with the symbol.</param>
        /// <param name="priority">The priority of the rule. Higher values are checked first.</param>
        public void Add(Regex regex, T value, int priority = DefaultPriority)
        {
            if (_sorted) throw new InvalidOperationException("Cannot add entries after context has been used.");
            if (regex == null) throw new ArgumentNullException("regex");
            if (_regexes.All(re => re.Item1 != regex)) _regexes.Add(Tuple.Create(regex, new RuleMatchValueGenerator<T>(value), priority));
        }

        /// <summary>
        /// Adds a regex rule to the context. This will throw an InvalidOperationException if called after the context is used to create tokens.
        /// </summary>
        /// <param name="regex">The regex to test for.</param>
        /// <param name="generator">A function that generates a token identifier from the match.</param>
        /// <param name="priority">The priority of the rule. Higher values are checked first.</param>
        public void Add(Regex regex, Func<Match, T> generator, int priority = DefaultPriority)
        {
            if (_sorted) throw new InvalidOperationException("Cannot add entries after context has been used.");
            if (regex == null) throw new ArgumentNullException("regex");
            if (generator == null) throw new ArgumentNullException("generator");
            if (_regexes.All(re => re.Item1 != regex)) _regexes.Add(Tuple.Create(regex, new RuleMatchValueGenerator<T>(generator), priority));
        }

        internal bool HasPunctuation(char c)
        {
            return _punctuation.Contains(c);
        }

        internal List<Tuple<Regex, RuleMatchValueGenerator<T>, int>> RegexList
        {
            get { return _regexes; }
        }

        private void Sort()
        {
            if (_sorted) return;
            _listNormal = _listNormal.OrderByDescending(t => t.Item1.Length).ToList();
            _listHigh = _listHigh.OrderByDescending(t => t.Item1.Length).ToList();
            _regexes = _regexes.OrderByDescending(r => r.Item3).ToList();
            _sorted = true;
        }

        internal List<Tuple<string, T>> NormalSymbols
        {
            get
            {
                Sort();
                return _listNormal;
            }
        }

        internal List<Tuple<string, T>> HighSymbols
        {
            get
            {
                Sort();
                return _listHigh;
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new InvalidOperationException("Cannot enumerate a rule set.");
        }
    }

    /// <summary>
    /// Used to manipulate the order in which symbol (non-regex) rules are tested.
    /// </summary>
    public enum LexerConstantPriority
    {
        /// <summary>
        /// Do not affect ordering.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Test symbol before testing any regex rules.
        /// </summary>
        High = 2
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