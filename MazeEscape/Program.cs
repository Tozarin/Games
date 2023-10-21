namespace MazeEscape
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var reader = new MazeReader(4);
            var maze = reader.ReadMaze("C:/test.maze");
            maze.PrintEdges();
        }
    }
}