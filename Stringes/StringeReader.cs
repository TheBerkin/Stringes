using System;
using System.Linq;
using System.Text.RegularExpressions;

using Stringes.Tokens;

namespace Stringes
{
    /// <summary>
    /// Represents a reader that can read data from a stringe.
    /// </summary>
    public sealed class StringeReader
    {
        private readonly Stringe _stringe;
        private int _pos;

        public StringeReader(string value)
        {
            _stringe = value.ToStringe();
            _pos = 0;
        }

        public StringeReader(Stringe value)
        {
            _stringe = value;
            _pos = 0;
        }

        public bool EndOfStringe
        {
            get { return _pos >= _stringe.Length; }
        }

        public Chare ReadChare()
        {
            return _stringe[_pos++];
        }

        public Chare PeekChare()
        {
            return EndOfStringe ? null : _stringe[_pos];
        }

        public Stringe ReadStringe(int length)
        {
            int p = _pos;
            _pos += length;
            return _stringe.Substringe(p, length);
        }

        public bool Eat(char value)
        {
            if (PeekChare() != value) return false;
            _pos++;
            return true;
        }

        public bool Eat(string value)
        {
            if (String.IsNullOrEmpty(value)) return false;
            if (_stringe.IndexOf(value, _pos) != _pos) return false;
            _pos += value.Length;
            return true;
        }

        public bool Eat(Regex regex)
        {
            if (regex == null) throw new ArgumentNullException("regex");
            var match = regex.Match(_stringe.Value, _pos);
            if (!match.Success || match.Index != _pos) return false;
            _pos += match.Length;
            return true;
        }

        public bool Eat(Regex regex, out Stringe result)
        {
            if (regex == null) throw new ArgumentNullException("regex");
            result = null;
            var match = regex.Match(_stringe.Value, _pos);
            if (!match.Success || match.Index != _pos) return false;
            result = _stringe.Substringe(_pos, match.Length);
            _pos += match.Length;
            return true;
        }

        public void SkipWhiteSpace()
        {
            while (!EndOfStringe && Char.IsWhiteSpace(_stringe.Value[_pos]))
            {
                _pos++;
            }
        }

        public bool EatToken<T>(LexerRules<T> tokenContext, out Token<T> token)
        {
            token = null;
            if (EndOfStringe) return false;
            
            // Check regex rules
            if (tokenContext.RegexList.Any())
            {
                Match longestMatch = null;
                var id = default(T);
                foreach (var re in tokenContext.RegexList)
                {
                    var match = re.Item1.Match(_stringe.Value, _pos);
                    if (match.Success && match.Index == _pos && (longestMatch == null || match.Length > longestMatch.Length))
                    {
                        longestMatch = match;
                        id = re.Item2.GetValue(match);
                    }
                }

                // If there was a match, generate a token.
                if (longestMatch != null)
                {
                    token = new Token<T>(id, _stringe.Substringe(longestMatch.Index, longestMatch.Length));
                    return true;
                }
            }

            // Check constant rules
            foreach (var t in tokenContext.Where(t => Eat(t.Item1)))
            {
                token = new Token<T>(t.Item2, t.Item1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The current zero-based position of the reader.
        /// </summary>
        public int Position
        {
            get { return _pos; }
            set
            {
                if (value < 0 || value > _stringe.Length)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _pos = value;
            }
        }

        /// <summary>
        /// The total length, in characters, of the stringe being read.
        /// </summary>
        public int Length
        {
            get { return _stringe.Length; }
        }
    }
}