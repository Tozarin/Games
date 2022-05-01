using NUnit.Framework;
using Discra;
using System.Collections.Generic;

namespace UnitTests
{
    public class GraphTests
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
        public void GetUnusedEdgesWhereVSourseTest()
        {
            var edges = new List<(int x, int y, int len)> { (1, 2, 10), (1, 3, 12)};
            var unused = new bool[] { false, true, false, false, false, false };
            Assert.AreEqual(graph.GetUnusedEdgesWhereVSourse(1, unused), edges);

            Assert.Pass();
        }

        [Test]
        public void AddEdgeTest()
        {
            var graph = new Graph(2);
            graph.AddEdge(0, 1, 1);
            graph.AddEdge((0, 0, 2));
            graph.AddEdge(1, 1, 2);

            var gr = new Graph(new int[,] { { 2, 1 }, { 1, 2 } });

            Assert.AreEqual(gr.Vertexes, graph.Vertexes);

            Assert.Pass();
        }

        [Test]
        public void GetVertexAndWeightWithMinValueTest()
        {
            var graph = new Graph(2);
            var used = new bool[] { false, false };
            graph.InitWeight();

            Assert.AreEqual(graph.GetVertexAndWeightWithMinValue(used), (0, 0));

            Assert.Pass();
        }

        [Test]
        public void UpdateWeightTest()
        {
            graph.UpdateWeight(0, 42);

            Assert.AreEqual(graph.Weights[0], 42);

            Assert.Pass();
        }

        [Test]
        public void GetMaxEdgeLenTest()
        {
            Assert.AreEqual(graph.GetMaxEdgeLen(), 20);

            Assert.Pass();
        }
    }
}
