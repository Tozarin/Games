namespace EnterpriseProgramming
{
    public class Algoritms
    {
        public static int FordFalkerson(List<List<int>> vertexes)
        {
            int size = vertexes.Count;
            bool[] isFree = new bool[size];

            int dfs(int s, int delta)
            {
                isFree[s] = false;
                if (s == size - 1)
                {
                    return delta;
                }
                for (int i = 0; i < size; i++)
                {
                    if (vertexes[s][i] != 0 && isFree[i])
                    {
                        int newDelta = dfs(i, Math.Min(delta, vertexes[s][i]));
                        if (newDelta > 0)
                        {
                            vertexes[s][i] -= newDelta;
                            vertexes[i][s] += newDelta;
                            return newDelta;
                        }
                    }
                }
                return 0;
            }

            int sum = 0;
            while (true)
            {
                isFree = isFree.Select(x => x = true).ToArray();
                int rez = dfs(0, int.MaxValue);
                if (rez > 0)
                {
                    sum += rez;
                }
                else
                {
                    break;
                }
            }

            return sum;
        }

        private struct Vertex
        {
            public int v;
            public int root;
            public int minDelta;

            public Vertex(int v, int root, int delta)
            {
                this.v = v;
                this.root = root;
                minDelta = delta;
            }
        }

        public static int EndmondsCarp(List<List<int>> vertexes)
        {
            int size = vertexes.Count;
            bool[] isFree = new bool[size];
            Vertex start = new Vertex(0, 0, int.MaxValue);

            List<Vertex> order = new List<Vertex> { start };
            int index = 0;

            void bfs(Vertex s)
            {
                if (s.v == size - 1)
                {
                    return;
                }
                for (int i = 0; i < size; i++)
                {
                    if (isFree[i] && vertexes[s.v][i] != 0)
                    {
                        isFree[i] = false;
                        order.Add(new Vertex(i, index, Math.Min(s.minDelta, vertexes[s.v][i])));
                    }
                }
                index++;
                if (index > order.Count - 1)
                {
                    return;
                }
                bfs(order[index]);
            }

            int sum = 0;
            while (true)
            {
                isFree = isFree.Select(x => x = true).ToArray();
                order = new List<Vertex> { start };
                index = 0;
                bfs(start);
                if (order.Count < size)
                {
                    break;
                }
                int delta = order[index].minDelta;
                //if (delta < 0)
                //{
                //    break;
                //}
                sum += delta;
                while (index != 0)
                {
                    Vertex curr = order[index];
                    vertexes[order[curr.root].v][curr.v] -= delta;
                    vertexes[curr.v][order[curr.root].v] += delta;
                    index = curr.root;
                }
            }
            return sum;
        }
    }
}
