namespace MazeEscape
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var maze = new Maze(4);
            maze = maze.ReadMaze("../../../test.maze");

            Console.WriteLine(maze.ContainsPoint((3.5f, 0.5f)));
        }
    }
}