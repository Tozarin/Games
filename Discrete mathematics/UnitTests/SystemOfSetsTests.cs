using NUnit.Framework;
using Discra;

namespace UnitTests
{
    public class SystemOfSetsTests
    {
        SystemOfSets sets = new SystemOfSets(10);

        [Test]
        public void FindSetTest()
        {
            Assert.AreEqual(sets.FindSet(1), 1);

            Assert.Pass();
        }

        [Test]
        public void UnionTest()
        {
            sets.Union(0, 1);

            Assert.AreEqual(sets.FindSet(1), 0);

            Assert.Pass();
        }
    }
}
