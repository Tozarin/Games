namespace Discra
{
    public class Search
    {
        public Graph Graph { get; private set; }
        private int[] layers;
        private bool[] marked;
        private int minFlow;

        public Search(Graph graph)
        {
            marked = new bool[graph.Size].Select(x => false).ToArray();
            layers = new int[graph.Size].Select(x => Int32.MaxValue).ToArray();
            Graph = new Graph(graph);
            minFlow = 1;
        }

        public Search(Graph graph, bool[] marked, int[] layers, int minFlow)
        {
            Graph = graph;
            this.layers = layers;
            this.marked = marked;
            this.minFlow = minFlow;
        }

        public Search WithMinFlow(int flow)
        {
            return new Search(Graph, marked, layers, flow);
        }

        public int FordFulkersonDfs(int s, int delta)
        {
            marked[s] = true;
            if (s == Graph.Size - 1)
            {
                return delta;
            }

            for (var i = 0; i < Graph.Size; i++)
            {
                if (!marked[i] && Math.Abs(Graph.Vertexes[s, i]) >= minFlow)
                {
                    var newDelta = FordFulkersonDfs(i, Math.Min(delta, Graph.Vertexes[s, i]));

                    if (newDelta > 0)
                    {
                        Graph.Vertexes[s, i] -= newDelta;
                        Graph.Vertexes[i, s] += newDelta;
                        return newDelta;
                    }
                }
            }

            return 0;
        }

        public int EdmondsKarpBfs(int s)
        {
            var way = new int[Graph.Size];
            var queue = new Queue<int>();
            queue.Enqueue(s);
            marked[s] = true;

            while (!marked[Graph.Size - 1] && queue.Count != 0)
            {
                var vv = queue.Dequeue();
                for (var i = 0; i < Graph.Size; i++)
                {
                    if (!marked[i] && Graph.Vertexes[vv, i] != 0)
                    {
                        queue.Enqueue(i);
                        way[i] = vv;
                        marked[i] = true;
                    }
                }
            }

            if (queue.Count == 0) return -1;

            var delta = Int32.MaxValue;
            var v = Graph.Size - 1;
            while (v != 0)
            {
                delta = Math.Min(delta, Graph.Vertexes[way[v], v]);
                v = way[v];
            }

            v = Graph.Size - 1;
            while (v != 0)
            {
                Graph.Vertexes[v, way[v]] += delta;
                Graph.Vertexes[way[v], v] -= delta;
                v = way[v];
            }

            return delta;
        }

        public bool DinicBfs(int s)
        {
            var queue = new Queue<int>();

            layers[s] = 0;
            queue.Enqueue(s);
            while (queue.Count != 0)
            {
                var v = queue.Dequeue();
                for (var i = 0; i < Graph.Size; i++)
                {
                    if (Graph.Vertexes[v, i] != 0 && layers[i] == Int32.MaxValue)
                    {
                        layers[i] = layers[v] + 1;
                        queue.Enqueue(i);
                    }
                }
            }

            return layers[Graph.Size - 1] != Int32.MaxValue;
        }

        public int DinicDfs(int s, int delta)
        {
            if (s == Graph.Size - 1 || delta == 0)
            {
                return delta;
            }

            for (int i = 0; i < Graph.Size; i++)
            {
                if (Graph.Vertexes[s, i] != 0 && layers[i] == layers[s] + 1)
                {
                    var newDelta = DinicDfs(i, Math.Min(delta, Graph.Vertexes[s, i]));
                    if (newDelta > 0)
                    {
                        Graph.Vertexes[s, i] -= newDelta;
                        Graph.Vertexes[i, s] += newDelta;
                        return newDelta;
                    }
                }
            }

            return 0;
        }
    }
}
