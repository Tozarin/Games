namespace Discra
{
    public class GraphAlgorithm
    {
        public Graph Kruskals(Graph graph)
        {
            var spanningTree = new Graph(graph.Size);
            var sets = new SystemOfSets(graph.Size);
            
            foreach (var e in graph.Edges)
            {
                var f = sets.FindSet(e.x);
                var s = sets.FindSet(e.y);
                if (f != s)
                {
                    sets.Union(f, s); 
                    spanningTree.AddEdge(e);
                }
            }

            return spanningTree;
        }

        public Graph Prim(Graph graph, int s)
        {
            var spanningTree = new Graph(graph.Size);
            var edges = new List<(int x, int y, int len)>(graph.Edges);
            var usedV = new bool[graph.Size];
            usedV[s] = true;
            var countOfV = 1;

            while(countOfV != graph.Size)
            {
                foreach(var e in edges)
                {
                    if (usedV[e.x] && !usedV[e.y])
                    {
                        edges.Remove(e);
                        spanningTree.AddEdge(e);
                        usedV[e.y] = true;
                        countOfV++;
                        break;
                    }
                }
            }

            return spanningTree;
        }

        public int[] Dijkstra(Graph graph)
        {
            var gr = new Graph(graph);
            var usedV = new bool[graph.Size];
            usedV[0] = true;
            var answer = new int[graph.Size];

            gr.InitWeight();

            for (var i = 0; i < gr.Size - 1; i++)
            {
                var v = gr.GetVertexAndWeightWithMinValue(usedV);
                var edges = gr.GetUnusedEdgesWhereVSourse(v.v, usedV);
                foreach (var e in edges)
                {
                    if (gr.Weights[e.y] > v.w + e.len)
                    {
                        gr.UpdateWeight(e.y, v.w + e.len);
                        answer[e.y] = v.v;
                    }
                }
                usedV[v.v] = true;
            }

            return answer;
        }

        public int FordFulkerson(Graph graph)
        {
            var gr = new Graph(graph);      

            var flow = 0;
            while (true)
            {
                var search = new Search(gr);
                var rezult = search.FordFulkersonDfs(0, Int32.MaxValue);
                gr = new Graph(search.Graph);

                if (rezult > 0) flow += rezult; else break;
            }

            return flow;
        }

        public int EdmondsKarp(Graph graph)
        {
            var gr = new Graph(graph);

            var flow = 0;
            while (true)
            {
                var search = new Search(gr);
                var rezult = search.EdmondsKarpBfs(0);
                gr = new Graph(search.Graph);

                if (rezult > 0) flow += rezult; else break;
            }

            return flow;
        }

        public int Dinic(Graph graph)
        {
            var search = new Search(graph);
            var flow = 0;
            while (search.DinicBfs(0))
            {
                var rezult = search.DinicDfs(0, Int32.MaxValue);
                while(rezult > 0)
                {
                    flow += rezult;
                    rezult = search.DinicDfs(0, Int32.MaxValue);
                }
                search = new Search(search.Graph);
            }

            return flow;
        }

        public int ScalingFlow(Graph graph)
        {
            var gr = new Graph(graph);
            var k = (int)(Math.Log2(gr.GetMaxEdgeLen()) + 1);

            var flow = 0;
            for (int i = k; i > 0; i--)
            {
                while (true)
                {
                    var search = new Search(gr).WithMinFlow(i);
                    var rezult = search.FordFulkersonDfs(0, Int32.MaxValue);
                    gr = new Graph(search.Graph);

                    if (rezult > 0) flow += rezult; else break;
                }
            }

            return flow;
        }
    }
}
