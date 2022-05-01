namespace Discra
{
    public class Graph
    {
        public int Size { get; }
        public int[] Weights { get; }
        public int[,] Vertexes { get; private set; }
        public List<(int x, int y, int len)> Edges { get; private set; } = new List<(int x, int y, int len)> ();

        public Graph(int size)
        {
            Size = size;
            Vertexes = new int[size, size];
            Weights = new int[size];
        }

        //public Graph(int size, int[,] vertexes, List<(int x, int y, int len)> edges, int[] weights)
        //{
        //    Size = size;
        //    Vertexes = vertexes;
        //    Edges = edges;
        //    Weights = weights;
        //}

        public Graph(Graph graph)
        {
            Size = graph.Size;
            Edges = new List<(int x, int y, int len)>(graph.Edges);
            Vertexes = (int[,])graph.Vertexes.Clone();
            Weights = (int[])graph.Weights.Clone();
        }

        public Graph(int[,] graph)
        {
            Size = graph.GetLength(0);
            Vertexes = graph;
            Weights = new int[Size];

            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    if (graph[i, j] != 0)
                    {
                        Edges.Add((i, j, graph[i, j]));
                    }
                }
            }

            Edges = Edges.OrderBy(x => x.len).ToList();
        }

        //public Graph(List<(int x, int y, int len)> graph)
        //{
        //    var v = new List<int>();
        //    Size = 0;
        //    foreach(var e in graph)
        //    {
        //        if (!v.Contains(e.x))
        //        {
        //            v.Add(e.x); 
        //            Size++;
        //        }
        //        if (!v.Contains(e.y))
        //        {
        //            v.Add(e.y); 
        //            Size++;
        //        }
        //    }

        //    Edges = graph.OrderBy(x => x.len).ToList();
        //    Vertexes = new int [Size, Size];
        //    Weights = new int[Size];

        //    foreach (var edge in Edges)
        //    {
        //        Vertexes[edge.x, edge.y] = edge.len;
        //    }
        //}

        //public Graph WithWeights(int[] weights)
        //{
        //    return new Graph(Size, Vertexes, Edges, weights);
        //}

        //public Graph WithEdges(List<(int x, int y, int len)> edges)
        //{
        //    return new Graph(Size, Vertexes, edges, Weights);
        //}

        public List<(int x, int y, int len)> GetUnusedEdgesWhereVSourse(int v, bool[] usedV)
        {
            var answer = new List<(int x, int y, int len)>();
            foreach (var e in Edges)
            {
                if (e.x == v && !usedV[e.y]) answer.Add(e);
            }

            return answer;
        }

        public void AddEdge((int x, int y, int len) edge)
        {
            Edges.Add(edge);
            Edges.Add((edge.y, edge.x, edge.len));
            Vertexes[edge.x, edge.y] = edge.len;
            Vertexes[edge.y, edge.x] = edge.len;
        }

        public void AddEdge(int x, int y, int len)
        {
            Edges.Add((x, y, len));
            Edges.Add((y, x, len));
            Vertexes[x, y] = len;
            Vertexes[y, x] = len;
        }

        public (int v, int w) GetVertexAndWeightWithMinValue(bool[] usedV)
        {
            var minW = Int32.MaxValue;
            var vertex = 0;

            for(var i = 0; i < Size; i++)
            {
                if (usedV[i]) continue;
                if (Weights[i] < minW)
                {
                    vertex = i;
                    minW = Weights[i];
                }
            }

            return (vertex, minW);
        }

        public void InitWeight()
        {
            Weights[0] = 0;
            for (var i = 1; i < Size; i++)
            {
                Weights[i] = Int32.MaxValue;
            }
        }

        public void UpdateWeight(int v, int w)
        {
            Weights[v] = w;
        }

        public int GetMaxEdgeLen()
        {
            return Edges.Last().len;
        }
    }
}
