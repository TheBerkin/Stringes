using System;
using System.Text.RegularExpressions;

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