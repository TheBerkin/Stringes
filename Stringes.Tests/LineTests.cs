using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Stringes.Tests
{
    [TestFixture]
    public class LineTests
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
    }
}
