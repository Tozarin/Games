namespace MazeEscape
{
    public class Maze
    {
        public int LenghtOfMaze { get; private set; }

        private FiguresManager _figuresManager;

        public Maze(int lenghtOfMaze)
        {
            LenghtOfMaze = lenghtOfMaze;
            _figuresManager = new FiguresManager();
        }

        public Maze WithLenghtOfMaze(int lenghtOfMaze) => new Maze(lenghtOfMaze);

        public void AddEdge(Edge edge) => _figuresManager.AddEdge(edge);

        public Maze ReadMaze(string fileName)
        {
            var reader = new MazeReader(LenghtOfMaze);
            return reader.ReadMaze(fileName);
        }
    }
}
