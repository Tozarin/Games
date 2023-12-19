using System.Text.RegularExpressions;

namespace MazeEscape
{
    public class MazeReaderV2
    {
        // Parse maze in format like:
        //
        //          0 0 - 0 1
        //          0 1 - 1 1
        //          1 1 - 1 0
        //          1 0 - 0 0
        //
        // that represens as:
        //
        //          # - #
        //          |   |
        //          # - #
        //
        // Don't protected to 0 0 - 1 1 situations


        public int LenghtOfMaze { get; private set; }

        public MazeReaderV2(int lenghtOfMaze)
        {
            if (lenghtOfMaze < 2)
            {
                throw new ArgumentException("Lenght of maze shoud be more that 1");
            }

            LenghtOfMaze = lenghtOfMaze;
        }

        public MazeReaderV2 WithLenghtOfMaze(int lenghtOfMaze) => new MazeReaderV2(lenghtOfMaze);

        public void AddEdge(string line, Maze maze)
        {
            var cords = line.Split(" ").Where(x => x != "-").Select(x => int.Parse(x)).ToList();
            var edge = new Edge(cords[0], cords[1], cords[2], cords[3]);


            if (edge.Points.Any(x => x.Item1 < 0 || x.Item1 >= LenghtOfMaze || x.Item2 < 0 || x.Item2 >= LenghtOfMaze))
            {
                throw new ArgumentException("Edge to put shoud be in maze's directions");
            }

            maze.AddEdge(edge);
        }

        public string ConvertEdge(Edge edge)
        {
            return edge.FirstPoint.Item1.ToString()
                + " " + edge.FirstPoint.Item2.ToString()
                + " - "
                + edge.SecondPoint.Item1.ToString()
                + " " + edge.SecondPoint.Item2.ToString();
        }

        public Maze ReadMaze(string fileName)
        {
            var maze = new Maze(LenghtOfMaze);
            var countOfReded = 0;

            using (var reader = new StreamReader(fileName))
            {
                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var pattern = new Regex(@"\d+\s\d+\s-\s\d+\s\d+");
                    var match = pattern.Match(line);

                    if (match.Success)
                        AddEdge(line, maze);
                    else
                        throw new Exception("Maze in incorrect format");

                    if (countOfReded++ % 1000 == 0)
                        Console.WriteLine("Readed " + countOfReded.ToString() + " edges");
                }
            }

            return maze;
        }
    }
}
