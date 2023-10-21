namespace MazeEscape
{
    public class Maze
    {
        public int LenghtOfMaze { get; private set; }


        private List<Edge> edgeList = new List<Edge>();
        public Maze(int lenghtOfMaze)
        {
            LenghtOfMaze = lenghtOfMaze;
        }

        public Maze WithLenghtOfMaze(int lenghtOfMaze) => new Maze(lenghtOfMaze);

        public void AddEdge(Edge edge)
        {
            edgeList.Add(edge);
        }

        public void PrintEdges()
        {
            foreach (var edge in edgeList)
            {
                Console.WriteLine(edge.FirstPoint.ToString() + " " + edge.SecondPoint.ToString());
            }
        }
    }
}
