namespace MazeEscape
{
    public class Cycle
    {
        private List<Edge> _edges;

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
            return null;
        }

        public List<List<Edge>> ExpandCycle(List<Edge> firstInners, List<Edge> secondInners, Edge edge)
        {
            return null;
        }

        //public void AddEdge(Edge edge) => _edges.Add(edge);

        public bool ContainsPointFromEdge(Edge edge)
            => _edges.Exists((x) => edge.IsConnectedWith(x));

        public bool ContainsFullEdge(Edge edge)
            => edge.Points.All((x) => ContainsPoint(x));

        public bool ContainsPoint((int, int) point)
        {
            return false; // TODO
        }
    }
}
