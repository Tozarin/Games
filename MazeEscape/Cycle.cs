namespace MazeEscape
{
    public class Cycle
    {
        private List<Edge> _edges;

        public int Lenght => _edges.Count;
        public List<Edge> Edges => new List<Edge> (_edges);

        public Cycle()
        {
            _edges = new List<Edge>();
        }

        public Cycle(List<Edge> edges)
        {
            _edges = edges;
        }

        private List<List<Edge>> GetTwoPathsFromInners(List<Edge> firstInners, List<Edge> secondInners)
        {
            var graph = new Graph();
            graph.FromEdges(_edges);

            var firstRoot = (-1, -1);
            var secondRoot = (-1, -1);
            foreach (var e in firstInners)
            {
                var root = graph.IsConnectedWithOn(e);

                if (root != (-1, -1))
                {
                    firstRoot = root;
                    break;
                }
            }

            foreach (var e in secondInners)
            {
                var root = graph.IsConnectedWithOn(e);

                if (root != (-1, -1))
                {
                    secondRoot = root;
                    break;
                }
            }

            if (firstRoot == (-1, -1) || secondRoot == (-1, -1))
                throw new Exception("No paths between inners");

            if (firstRoot == secondRoot)
                return new List<List<Edge>> { firstInners };

            return graph.FindTwoPaths(firstRoot, secondRoot);
        }

        public List<List<Edge>> FromTwoCyclesAndInners(Cycle secondCycle, List<Edge> firstInners, List<Edge> secondInners)
        {
            var paths = GetTwoPathsFromInners(firstInners, secondInners);

            var firstInnersWithPath = firstInners.Concat(paths.First()).ToList();
            var secondInnersWithPath = firstInners.Concat(paths.Last()).ToList();

            var tmpCycle = new Cycle(secondCycle.Edges);
            var newInners = tmpCycle.ExpandCycle(firstInnersWithPath, secondInners);

            if (tmpCycle.ContainsFullEdge(paths.Last().First()))
            {
                _edges = tmpCycle.Edges;
                return newInners;
            }

            tmpCycle = new Cycle(secondCycle.Edges);
            newInners = tmpCycle.ExpandCycle(secondInnersWithPath, secondInners);

            _edges = tmpCycle.Edges;
            return newInners;
        }

        public List<List<Edge>> FromSingleInner(List<Edge> inner)
        {
            var graph = new Graph();
            graph.FromEdges(inner);

            var (cycle, inners) = graph.GetCycleAndInners();
            _edges = cycle;

            inners = inners
                .Select((x) => x.Where((y) => !ContainsFullEdge(y)).ToList())
                .Where((x) => x.Count > 0)
                .ToList();

            return inners;
        }

        public List<List<Edge>> ExpandCycle(List<Edge> firstInners, List<Edge> secondInners)
        {
            var paths = GetTwoPathsFromInners(firstInners, secondInners);

            var newCycle = new Cycle();
            var commonPart = new List<Edge>().Concat(firstInners).Concat(secondInners).ToList();
            var newInners = newCycle.FromSingleInner(commonPart.Concat(paths.First()).ToList());

            if (!newCycle.ContainsFullEdge(paths.Last().First()))
            {
                newCycle = new Cycle();
                newInners = newCycle.FromSingleInner(commonPart.Concat(paths.Last()).ToList());
            }

            _edges = newCycle.Edges;

            return newInners;
        }

        //public void AddEdge(Edge edge) => _edges.Add(edge);

        public bool ContainsPointFromEdge(Edge edge)
            => _edges.Exists((x) => edge.IsConnectedWith(x));

        public bool ContainsFullEdge(Edge edge)
            => edge.Points.All((x) => ContainsPoint(((float)x.Item1, (float)x.Item2)));

        public bool ContainsPoint((float, float) point)
        {
            var x = point.Item1;
            var y = point.Item2;

            var countOfCross = 0;

            foreach(var edge in _edges)
            {
                var (x1, y1) = edge.FirstPoint;
                var (x2, y2) = edge.SecondPoint;

                if ((x == x1 && y == y1) || (x == x2 && y == y2))
                    return true;

                //if (y1 == y && y2 == y)
                //    if (x <= x1 && x <= x2)
                //        continue;

                if (x <= x1 && x <= x2)
                    if ((y1 <= y && y < y2) || (y2 <= y && y < y1))
                        countOfCross++;
            }

            return countOfCross % 2 == 1;
        }
    }
}
