namespace MazeEscape
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var maze = new Maze(4);
            maze = maze.ReadMaze("C:/test.maze");

            Console.WriteLine(maze.ContainsPoint((0.5f, 0.5f)));
            Console.WriteLine(maze.ContainsPoint((3.5f, 0.5f)));
            Console.WriteLine(maze.ContainsPoint((1.5f, 3.5f)));
            Console.WriteLine(maze.ContainsPoint((0.5f, 2.5f)));
        }
    }
}