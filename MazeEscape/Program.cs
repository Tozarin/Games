namespace MazeEscape
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var maze = new Maze(4);
            maze = maze.ReadMaze("../../../test2.maze");

            Console.WriteLine(!maze.ContainsPoint((0.5f, 0.5f)));

            var maze2 = new Maze(1000000);
            maze2 = maze2.ReadMazeV2("../../../test.maze");

            Console.WriteLine(!maze2.ContainsPoint((0.5f, 0.5f)));
        }
    }
}