﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Stringes
{
    /// <summary>
    /// Describes a string or a substring in relation to its parent. Provides line number, column, offset, and other useful metadata.
    /// </summary>
    public sealed class Stringe : IEnumerable<Chare>
    {
        private readonly Stref _stref;
        private readonly int _offset;
        private readonly int _length;
        private readonly int _line;
        private readonly int _column;
        private string _substring;

        /// <summary>
        /// Returns an empty stringe based on the position of another stringe.
        /// </summary>
        /// <param name="basis">The basis stringe to get position info from.</param>
        /// <returns></returns>
        public static Stringe Empty(Stringe basis)
        {
            return new Stringe(basis, 0, 0);
        }

        /// <summary>
        /// Indicates whether the specified stringe is null or empty.
        /// </summary>
        /// <param name="stringe">The stringe to test.</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(Stringe stringe)
        {
            return stringe == null || stringe.Length == 0;
        }

        /// <summary>
        /// The offset of the stringe in the string.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// The length of the string represented by the stringe.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// The 1-based line number at which the stringe begins.
        /// </summary>
        public int Line
        {
            get { return _line; }
        }

        /// <summary>
        /// The 1-based column at which the stringe begins.
        /// </summary>
        public int Column
        {
            get { return _column; }
        }

        /// <summary>
        /// Indicates if the stringe is a substring.
        /// </summary>
        public bool IsSubstring
        {
            get { return _offset > 0 || _length < _stref.String.Length; }
        }

        /// <summary>
        /// Indicates if the stringe is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _length == 0; }
        }

        /// <summary>
        /// The substring value represented by the stringe. If the stringe is the parent, this will provide the original string.
        /// </summary>
        public string Value
        {
            // Lazily evaluated.
            get { return _substring ?? (_substring = ToString()); }
        }

        /// <summary>
        /// Gets the original string from which the stringe was originally derived.
        /// </summary>
        public string ParentString
        {
            get { return _stref.String; }
        }

        /// <summary>
        /// Creates a new stringe from the specified string.
        /// </summary>
        /// <param name="value">The string to turn into a stringe.</param>
        public Stringe(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            _stref = new Stref(value);
            _offset = 0;
            _length = value.Length;
            _line = 1;
            _column = 1;
            _substring = null;
        }

        private Stringe(Stringe parent, int relativeOffset, int length)
        {
            _stref = parent._stref;
            _offset = parent._offset + relativeOffset;
            _length = length;
            _substring = null;

            // Calculate line/col
            _line = parent._line;
            _column = parent._column;

            if (relativeOffset <= 0) return; // Do nothing if the offset is the same

            for (int i = 0; i < relativeOffset; i++)
            {
                if (_stref.String[parent._offset + i] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else if (_stref.Bases[i]) // Advance column only for non-combining characters
                {
                    _column++;
                }
            }
        }

        /// <summary>
        /// Gets the charactere at the specified index in the stringe.
        /// </summary>
        /// <param name="index">The index of the charactere to retrieve.</param>
        /// <returns></returns>
        public Chare this[int index]
        {
            get
            {
                return _stref.Chares[index] ?? (_stref.Chares[index] = new Chare(this, _stref.String[index], index + _offset));
            }
        }

        /// <summary>
        /// Returns the zero-based index at which the specified string first occurs, relative to the substringe. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The string to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <param name="comparisonType">The string comparison rules to apply to the search.</param>
        /// <returns></returns>
        public int IndexOf(string input, int start = 0, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return Value.IndexOf(input, start, comparisonType);
        }

        /// <summary>
        /// Returns the zero-based index at which the specified string first occurs, relative to the parent string. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The string to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <param name="comparisonType">The string comparison rules to apply to the search.</param>
        /// <returns></returns>
        public int IndexOfTotal(string input, int start = 0, StringComparison comparisonType = StringComparison.Ordinal)
        {
            int index = Value.IndexOf(input, start, comparisonType);
            return index == -1 ? index : index + _offset;
        }

        /// <summary>
        /// Returns the zero-based index at which the specified character first occurs, relative to the substringe. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The character to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <returns></returns>
        public int IndexOf(char input, int start = 0)
        {
            return Value.IndexOf(input, start);
        }

        /// <summary>
        /// Returns the zero-based index at which the specified character first occurs, relative to the parent string. The search starts at the specified index.
        /// </summary>
        /// <param name="input">The character to search for.</param>
        /// <param name="start">The index at which to begin the search.</param>
        /// <returns></returns>
        public int IndexOfTotal(char input, int start = 0)
        {
            int index = Value.IndexOf(input, start);
            return index == -1 ? index : index + _offset;
        }

        /// <summary>
        /// Creates a substringe from the stringe, starting at the specified index and extending to the specified length.
        /// </summary>
        /// <param name="offset">The offset at which to begin the substringe.</param>
        /// <param name="length">The length of the substringe.</param>
        /// <returns></returns>
        public Stringe Substringe(int offset, int length)
        {
            return new Stringe(this, offset, length);
        }

        /// <summary>
        /// Create a substringe from the stringe, starting at the specified index and extending to the end.
        /// </summary>
        /// <param name="offset">The offset at which to begin the substringe.</param>
        /// <returns></returns>
        public Stringe Substringe(int offset)
        {
            return new Stringe(this, offset, Length - offset);
        }


        /// <summary>
        /// Returns the stringe with all leading and trailing white space characters removed.
        /// </summary>
        /// <returns></returns>
        public Stringe Trim()
        {
            if (_length == 0) return this;
            int a = 0;
            int b = _length;
            do
            {
                if (Char.IsWhiteSpace(Value[a]))
                {
                    a++;
                }
                else if (Char.IsWhiteSpace(Value[b - 1]))
                {
                    b--;
                }
                else
                {
                    break;
                }
            } while (a < b && b > 0 && a < _length);

            return Substringe(a, b - a);
        }

        public Stringe Trim(params char[] trimChars)
        {
            if (_length == 0) return this;
            bool useDefault = trimChars.Length == 0;
            int a = 0;
            int b = _length;
            do
            {
                if (useDefault ? Char.IsWhiteSpace(Value[a]) : trimChars.Contains(Value[a]))
                {
                    a++;
                }
                else if (useDefault ? Char.IsWhiteSpace(Value[b - 1]) : trimChars.Contains(Value[b - 1]))
                {
                    b--;
                }
                else
                {
                    break;
                }
            } while (a < b && b > 0 && a < _length);

            return Substringe(a, b - a);
        }

        public Stringe TrimStart(params char[] trimChars)
        {
            if (_length == 0) return this;
            bool useDefault = trimChars.Length == 0;
            int a = 0;
            while (a < _length)
            {
                if (useDefault ? Char.IsWhiteSpace(Value[a]) : trimChars.Contains(Value[a]))
                {
                    a++;
                }
                else
                {
                    break;
                }
            }
            return Substringe(a);
        }

        public Stringe TrimEnd(params char[] trimChars)
        {
            if (_length == 0) return this;
            bool useDefault = trimChars.Length == 0;
            int b = _length;
            do
            {
                if (useDefault ? Char.IsWhiteSpace(Value[b - 1]) : trimChars.Contains(Value[b - 1]))
                {
                    b--;
                }
                else
                {
                    break;
                }
            } while (b > 0);
            return Substringe(0, b);
        }

        public IEnumerable<Stringe> Split(char[] separators, StringSplitOptions options = StringSplitOptions.None)
        {
            int start = 0;
            for (int i = 0; i < _length; i++)
            {
                if (!separators.Contains(Value[i])) continue;
                if (options == StringSplitOptions.None || i - start > 0) yield return Substringe(start, i - start);
                start = i + 1;
            }
            if (start > _length) yield break;
            if (options == StringSplitOptions.None || _length - start > 0) yield return Substringe(start, _length - start);
        }

        public IEnumerable<Stringe> Split(char[] separators, int count, StringSplitOptions options = StringSplitOptions.None)
        {
            if (count == 0) yield break;
            if (count == 1)
            {
                yield return this;
                yield break;
            }

            int matches = 0;
            int start = 0;

            for (int i = 0; i < _length; i++)
            {
                if (separators.Contains(Value[i]))
                {
                    if (options == StringSplitOptions.None || i - start > 0) yield return Substringe(start, i - start);
                    start = i + 1;
                    matches++;
                }
                if (matches < count - 1) continue;
                if (start > _length) yield break;
                yield return Substringe(start, _length - start);
                yield break;
            }
            if (start > _length || matches >= count) yield break;
            if (options == StringSplitOptions.None || _length - start > 0) yield return Substringe(start, _length - start);
        }

        public static explicit operator string(Stringe stringe)
        {
            return stringe._stref.String.Substring(stringe._offset, stringe._length);
        }        

        public override string ToString()
        {
            return _stref.String.Substring(_offset, _length);
        }

        /// <summary>
        /// Stores a reference to a string, to prevent unnecessary copies being created.
        /// </summary>
        private class Stref
        {
            public readonly string String;
            public readonly Chare[] Chares;
            public readonly bool[] Bases;

            public Stref(string str)
            {
                String = str;
                Chares = new Chare[str.Length];
                Bases = new bool[str.Length];
                var elems = StringInfo.GetTextElementEnumerator(str);
                while (elems.MoveNext())
                {
                    Bases[elems.ElementIndex] = true;
                }
            }
        }

        private IEnumerable<Chare> _Chares()
        {
            for (int i = 0; i < _length; i++)
            {
                yield return this[i];
            }
        }

        public IEnumerator<Chare> GetEnumerator()
        {
            return _Chares().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
