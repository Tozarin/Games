using NUnit.Framework;
using Discra;

namespace UnitTests
{
    public class GraphAlgorithmsTests
    {
        [Test]
        public void KruskalsTest()
        {
            var vertexesForGraph = new int[,]
            {
                { 0, 7, 8, 0, 0, 0 },
                { 7, 0, 11, 2, 0, 0 },
                { 8, 11, 0, 6, 9, 0 },
                { 0, 2, 6, 0, 11, 9 },
                { 0, 0, 9, 11, 0, 10 },
                { 0, 0, 0, 9, 10, 0}
            };
            var vertexesForSpanTree = new int[,]
            {
                { 0, 7, 0, 0, 0, 0 },
                { 7, 0, 0, 2, 0, 0 },
                { 0, 0, 0, 6, 9, 0 },
                { 0, 2, 6, 0, 0, 9 },
                { 0, 0, 9, 0, 0, 0 },
                { 0, 0, 0, 9, 0, 0 }
            };

            var graph = new Graph(vertexesForGraph);
            var spanTree = new Graph(vertexesForSpanTree);
            graph = new GraphAlgorithm().Kruskals(graph);

            Assert.AreEqual(graph.Vertexes, spanTree.Vertexes);

            Assert.Pass();
        }

        [Test]
        public void PrimTest()
        {
            var vertexesForGraph = new int[,]
            {
                { 0, 7, 8, 0, 0, 0 },
                { 7, 0, 11, 2, 0, 0 },
                { 8, 11, 0, 6, 9, 0 },
                { 0, 2, 6, 0, 11, 9 },
                { 0, 0, 9, 11, 0, 10 },
                { 0, 0, 0, 9, 10, 0}
            };
            var vertexesForSpanTree = new int[,]
            {
                { 0, 7, 0, 0, 0, 0 },
                { 7, 0, 0, 2, 0, 0 },
                { 0, 0, 0, 6, 9, 0 },
                { 0, 2, 6, 0, 0, 9 },
                { 0, 0, 9, 0, 0, 0 },
                { 0, 0, 0, 9, 0, 0 }
            };

            var graph = new Graph(vertexesForGraph);
            var spanTree = new Graph(vertexesForSpanTree);
            graph = new GraphAlgorithm().Prim(graph, 4);

            Assert.AreEqual(graph.Vertexes, spanTree.Vertexes);

            Assert.Pass();
        }

        [Test]
        public void DijkstraTest()
        {
            var vertexesForGraph = new int[,]
            {
                { 0, 7, 8, 0, 0, 0 },
                { 7, 0, 11, 2, 0, 0 },
                { 8, 11, 0, 6, 9, 0 },
                { 0, 2, 6, 0, 11, 9 },
                { 0, 0, 9, 11, 0, 10 },
                { 0, 0, 0, 9, 10, 0}
            };
            
            var graph = new Graph(vertexesForGraph);
            
            Assert.AreEqual(new GraphAlgorithm().Dijkstra(graph), new int[] { 0, 0, 0, 1, 2, 3});

            Assert.Pass();
        }

        [Test]
        public void FordFulkersonTest()
        {
            var vertexes = new int[,]
            {
                { 0, 16, 13, 0, 0, 0 },
                { 0, 0, 10, 12, 0, 0 },
                { 0, 4, 0, 0, 14, 0 },
                { 0, 0, 9, 0, 0, 20 },
                { 0, 0, 0, 7, 0, 4 },
                { 0, 0, 0, 0, 0, 0}
            };
            var graph = new Graph(vertexes);

            Assert.AreEqual(new GraphAlgorithm().FordFulkerson(graph), 23);

            Assert.Pass();
        }

        [Test]
        public void EdmondsKarpTest()
        {
            var vertexes = new int[,]
            {
                { 0, 16, 13, 0, 0, 0 },
                { 0, 0, 10, 12, 0, 0 },
                { 0, 4, 0, 0, 14, 0 },
                { 0, 0, 9, 0, 0, 20 },
                { 0, 0, 0, 7, 0, 4 },
                { 0, 0, 0, 0, 0, 0}
            };
            var graph = new Graph(vertexes);

            Assert.AreEqual(new GraphAlgorithm().EdmondsKarp(graph), 23);

            Assert.Pass();
        }

        [Test]
        public void DinicTest()
        {
            var vertexes = new int[,]
            {
               { 0, 16, 13, 0, 0, 0 },
                { 0, 0, 10, 12, 0, 0 },
                { 0, 4, 0, 0, 14, 0 },
                { 0, 0, 9, 0, 0, 20 },
                { 0, 0, 0, 7, 0, 4 },
                { 0, 0, 0, 0, 0, 0}
            };
            var graph = new Graph(vertexes);

            Assert.AreEqual(new GraphAlgorithm().Dinic(graph), 23);

            Assert.Pass();
        }

        [Test]
        public void ScalingFlowTest()
        {
            var vertexes = new int[,]
            {
                { 0, 16, 13, 0, 0, 0 },
                { 0, 0, 10, 12, 0, 0 },
                { 0, 4, 0, 0, 14, 0 },
                { 0, 0, 9, 0, 0, 20 },
                { 0, 0, 0, 7, 0, 4 },
                { 0, 0, 0, 0, 0, 0}
            };
            var graph = new Graph(vertexes);

            Assert.AreEqual(new GraphAlgorithm().ScalingFlow(graph), 23);

            Assert.Pass();
        }
    }
}