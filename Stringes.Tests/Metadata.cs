using System;

using NUnit.Framework;

namespace Stringes.Tests
{
    [TestFixture]
    public class Metadata
    {
        [TestCase("no no no no no", 0, 2, 5)]
        [TestCase("a b c d e f g h", 0, 1, 1)]
        public void Occurrences(string parent, int subIndex, int subLength, int expectedCount)
        {
            var streParent = parent.ToStringe();
            var streSub = streParent.Substringe(subIndex, subLength);
            Assert.AreEqual(expectedCount, streSub.OccurrenceCount);
        }
    }
}