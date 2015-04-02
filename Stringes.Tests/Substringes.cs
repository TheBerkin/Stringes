using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Stringes.Tests
{
    [TestFixture]
    public class Substringes
    {
        [TestCase("Hello\nWorld", "Hello", "World")]
        [TestCase("A\n", "A", "")]
        [TestCase("\nB", "", "B")]
        public void TwoLineSplit(string parent, string line1, string line2)
        {
            var split = parent.ToStringe().Split('\n').ToArray();

            Assert.IsTrue(split.Length == 2);
            Assert.AreEqual(line1, split[0].Value);
            Assert.AreEqual(line2, split[1].Value);
        }

        [TestCase("The quick brown fox jumps over the lazy dog", "The", 0, 3, "fox", 16, 3, "The quick brown fox")]
        public void RangeText(string parent,
            string a, int indexA, int lengthA,
            string b, int indexB, int lengthB,
            string expectedRange)
        {
            var streParent = parent.ToStringe();
            var streA = streParent.Substringe(indexA, lengthA);
            var streB = streParent.Substringe(indexB, lengthB);
            var streBetween = Stringe.Range(streA, streB);
            Assert.AreEqual(a, streA.Value);
            Assert.AreEqual(b, streB.Value);
            Assert.AreEqual(expectedRange, streBetween.Value);
        }
    }
}
