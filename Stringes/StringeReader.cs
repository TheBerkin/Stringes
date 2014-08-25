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

        public bool IsNext(char value)
        {
            return PeekChare() == value;
        }

        public bool IsNext(string value)
        {
            if (String.IsNullOrEmpty(value)) return false;
            return _stringe.IndexOf(value, _pos) == _pos;
        }

        public bool IsNext(Regex regex)
        {
            if (regex == null) throw new ArgumentNullException("regex");
            var match = regex.Match(_stringe.Value, _pos);
            return match.Success && match.Index == _pos;
        }

        public bool IsNext(Regex regex, out Stringe result)
        {
            if (regex == null) throw new ArgumentNullException("regex");
            result = null;
            var match = regex.Match(_stringe.Value, _pos);
            if (!match.Success || match.Index != _pos) return false;
            result = _stringe.Substringe(_pos, match.Length);
            return true;
        }

        public void SkipWhiteSpace()
        {
            while (!EndOfStringe && Char.IsWhiteSpace(_stringe.Value[_pos]))
            {
                _pos++;
            }
        }

        public Token<T> EatToken<T>(LexerRules<T> rules)
        {
            if (EndOfStringe)
            {
                if (rules.EndToken != null)
                {
                    return new Token<T>(rules.EndToken.Item2, _stringe.Substringe(_pos, 0));
                }

                throw new InvalidOperationException("Unexpected end of input.");
            }

            // Indicates if undefined tokens should be created
            bool captureUndef = rules.UndefinedCaptureRule != null;

            // Tracks the beginning of the undefined token content
            int u = _pos;

            do
            {
                // If we've reached the end, return undefined token, if present.
                if (EndOfStringe && captureUndef && u < _pos)
                {
                    return new Token<T>(rules.UndefinedCaptureRule.Item2, rules.UndefinedCaptureRule.Item1(_stringe.Slice(u, _pos)));
                }

                // Check high priority symbol rules
                foreach (var t in rules.HighSymbols.Where(t => IsNext(t.Item1)))
                {
                    // Return undefined token if present
                    if (captureUndef && u < _pos)
                    {
                        return new Token<T>(rules.UndefinedCaptureRule.Item2, rules.UndefinedCaptureRule.Item1(_stringe.Slice(u, _pos)));
                    }

                    // Return symbol token
                    var c = _stringe.Substringe(_pos, t.Item1.Length);
                    _pos += t.Item1.Length;
                    return new Token<T>(t.Item2, c);
                }

                // Check regex rules
                if (rules.RegexList.Any())
                {
                    Match longestMatch = null;
                    var id = default(T);

                    // Find the longest match, if any.
                    foreach (var re in rules.RegexList)
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
                        // Return undefined token if present
                        if (captureUndef && u < _pos)
                        {
                            return new Token<T>(rules.UndefinedCaptureRule.Item2, rules.UndefinedCaptureRule.Item1(_stringe.Slice(u, _pos)));
                        }

                        // Return longest match.
                        _pos += longestMatch.Length;
                        return new Token<T>(id, _stringe.Substringe(longestMatch.Index, longestMatch.Length));
                    }
                }

                // Check normal priority symbol rules
                foreach (var t in rules.NormalSymbols.Where(t => IsNext(t.Item1)))
                {
                    // Return undefined token if present
                    if (captureUndef && u < _pos)
                    {
                        return new Token<T>(rules.UndefinedCaptureRule.Item2, rules.UndefinedCaptureRule.Item1(_stringe.Slice(u, _pos)));
                    }

                    // Return symbol token
                    var c = _stringe.Substringe(_pos, t.Item1.Length);
                    _pos += t.Item1.Length;
                    return new Token<T>(t.Item2, c);
                }

                _pos++;

                if(!captureUndef)
                {
                    var bad = _stringe.Slice(u, _pos);
                    throw new InvalidOperationException(String.Concat("(Ln ", bad.Line, ", Col ", bad.Column, ") Invalid token '", bad, "'"));
                }

            } while (captureUndef);

            throw new InvalidOperationException("This should never happen.");
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