using NUnit.Framework;
using Discra;
using System;

namespace UnitTests
{
    public class SearchTests
    {
        static int[,] vertexes = new int[,]
            {
                { 0, 16, 13, 0, 0, 0 },
                { 0, 0, 10, 12, 0, 0 },
                { 0, 4, 0, 0, 14, 0 },
                { 0, 0, 9, 0, 0, 20 },
                { 0, 0, 0, 7, 0, 4 },
                { 0, 0, 0, 0, 0, 0}
            };
        Graph graph = new Graph(vertexes);

        [Test]
        public void FordFulkersonDfsTest()
        {
            Assert.AreEqual(new Search(graph).FordFulkersonDfs(0, Int32.MaxValue), 7);

            Assert.Pass();
        }

        [Test]
        public void EdmondsKarpBfsTest()
        {
            Assert.AreEqual(new Search(graph).EdmondsKarpBfs(0), 12);

            Assert.Pass();
        }

        [Test]
        public void DinicBfsTest()
        {
            Assert.IsTrue(new Search(graph).DinicBfs(0));

            Assert.Pass();
        }

        [Test]
        public void DinicDfsTest()
        {
            var search = new Search(graph);
            search.DinicBfs(0);

            Assert.AreEqual(search.DinicDfs(0, Int32.MaxValue), 12);

            Assert.Pass();
        }
    }
}
