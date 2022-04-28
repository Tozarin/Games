namespace Discra
{
    public class Alhoritms
    {
        public List<List<int>> Kraskal(List<List<int>> graf)
        {
            List<List<int>> newGraf = new List<List<int>>();
            for (int i = 0; i < graf.Count; i++)
            {
                List<int> list = new List<int>();
                for (int j = 0; j < graf[i].Count; j++)
                    list.Add(0);
                newGraf.Add(list);
            }
            List<(int x, int y, int d)> edges = new List<(int x, int y, int d)>();
            List<int> usedV = new List<int>();

            for(int i = 0; i < graf.Count; i++)
            {
                for(int j = 0; j < graf[i].Count; j++)
                {
                    if (graf[i][j] != 0) edges.Add((i, j, graf[i][j]));
                }
            }

            edges = edges.OrderBy(x => x.d).ToList();
            foreach(var e in edges)
            {
                if (!usedV.Contains(e.x) && usedV.Contains(e.y) || usedV.Contains(e.x) && !usedV.Contains(e.y)
                    || !usedV.Contains(e.x) && !usedV.Contains(e.y))
                {
                    usedV.Add(e.x);
                    usedV.Add(e.y);
                    newGraf[e.x][e.y] = e.d;
                    newGraf[e.y][e.x] = e.d;
                }
            }

            return newGraf;
        }

        public List<List<int>> Prim(List<List<int>> graf, int v)
        {
            List<List<int>> newGraf = new List<List<int>>();
            for (int i = 0; i < graf.Count; i++)
            {
                List<int> list = new List<int>();
                for (int j = 0; j < graf[i].Count; j++)
                    list.Add(0);
                newGraf.Add(list);
            }
            List<int> usedV = new List<int> { v };

            while (usedV.Count != graf[0].Count)
            {
                int minE = Int32.MaxValue;
                int toV = 0;
                int fromV = 0;
                foreach (int vv in usedV)
                {
                    for (int e = 0; e < graf[vv].Count; e++)
                    {
                        if (graf[vv][e] != 0 && !usedV.Contains(e))
                        {
                            if (minE > graf[vv][e])
                            {
                                minE = graf[vv][e];
                                toV = e;
                                fromV = vv;
                            }
                        }
                    }
                }

                usedV.Add(toV);
                newGraf[fromV][toV] = minE;
                newGraf[toV][fromV] = minE;
            }

            return newGraf;
        }

        public List<int> Decsra(List<List<int>> graf, int v)
        {
            List<int> usedV = new List<int> { v };
            List<bool> usd = new List<bool> { false };
            List<int> weights = new List<int> { 0 };
            List<int> answer = new List<int> { v };
            for (int _ = 0; _ < graf[0].Count - 1; _++)
            {
                weights.Add(Int32.MaxValue);
                answer.Add(v);
                usd.Add(true);
            }

            while (usedV.Count != graf[0].Count)
            {
                int minW = Int32.MaxValue;
                int vv = 0;
                for (int i = 0; i < weights.Count; i++)
                {
                    if (weights[i] < minW)
                    {
                        minW = weights[i];
                        vv = i;
                    }
                }

                for (int i = 0; i < graf[vv].Count; i++)
                {
                    if (graf[vv][i] != 0 && usd[i])
                    {
                        if (minW + graf[vv][i] < weights[i])
                        {
                            weights[i] = minW + graf[vv][i];
                            usedV.Add(i);
                            usd[i] = false;
                            answer[i] = vv;
                        }
                    }
                }
            }

            return answer;
        }
    }
}
