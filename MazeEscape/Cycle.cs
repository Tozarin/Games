namespace MazeEscape
{
    public class Cycle
    {
        private List<Edge> _edges;

        public int Lenght => _edges.Count;

        public Cycle()
        {
            _edges = new List<Edge>();
        }

        public Cycle FromTwoCyclesAndInners(Cycle secondCycle, List<Edge> firstInner, List<Edge> secondInner)
        {
            return null;
        }

        public List<List<Edge>> FromSingleInner(List<Edge> inner)
        {
            var graph = new Graph();
            graph.FromEdges(inner);

            var (cycle, inners) = graph.GetCycleAndInners();
            _edges = cycle;

            inners = inners.Select((x) => x.Where((y) => !ContainsFullEdge(y)).ToList()).ToList();

            return inners;
        }

        public List<List<Edge>> ExpandCycle(List<Edge> firstInners, List<Edge> secondInners, Edge edge)
        {
            return null;
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

                if (y1 >= y && y2 >= y)
                    if ((x1 <= x && x <= x2) || (x2 <= x && x <= x1))
                        countOfCross++;
            }

            return countOfCross % 2 == 1;
        }
    }
}
