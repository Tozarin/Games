using System.Text.RegularExpressions;

namespace MazeEscape
{
    public class MazeReader
    {
        // Parse maze in format like:
        //
        //          # = # = #   # = # = #
        //          |       |           |
        //          #   # = # = # = #   #
        //          |           |   |    
        //          #   #   #   # = #   #
        //          |       |   |   |   |
        //          #   # = #   # = #   #
        //          |       |           |
        //          # = #   # = # = # = #
        //              |   |           |
        //          #   # = # = # = #   #


        public int LenghtOfMaze { get; private set; }

        private const string _pointMazePattern = "#";
        private const string _verticalLinePattern = "|";
        private const string _horizontalLinePattern = "=";
       
        private const string _evenMazePattern = _pointMazePattern + @"\s[" + _horizontalLinePattern + @"\s]\s";
        private const string _oddMazePattern = "[" + _verticalLinePattern + @"\s]\s\s\s";

        private string _mainEvenMazePattern;
        private string _mainOddMazePattern;

        public MazeReader(int lenghtOfMaze)
        {
            if (lenghtOfMaze < 2)
            {
                throw new ArgumentException("Lenght of maze shoud be more that 1");
            }

            LenghtOfMaze = lenghtOfMaze;

            _mainEvenMazePattern = new string(' ', lenghtOfMaze).Replace(" ", _evenMazePattern) + _pointMazePattern;
            _mainOddMazePattern = new string(' ', lenghtOfMaze).Replace(" ", _oddMazePattern) + @"[\s" + _verticalLinePattern + "]";
        }

        public MazeReader WithLenghtOfMaze(int lenghtOfMaze) => new MazeReader(lenghtOfMaze);

        private void AddEvenLine(string line, int countOfLines, Maze maze)
        {
            var splittedLine = line.ToCharArray().Select(x => x.ToString()).ToArray();

            for (var position = 2; position < splittedLine.Length; position += 4)
            {
                if (splittedLine[position] == _horizontalLinePattern)
                {
                    var edge = new Edge(position / 4, countOfLines / 2, position / 4 + 1, countOfLines / 2);
                    maze.AddEdge(edge);
                }
            }

        }

        private void AddOddLine(string line, int countOfLines, Maze maze)
        {
            var splittedLine = line.ToCharArray().Select(x => x.ToString()).ToArray();

            for (var position = 0; position < line.Length; position += 4)
            {
                if (splittedLine[position] == _verticalLinePattern)
                {
                    var edge = new Edge(position / 4, countOfLines / 2, position / 4, countOfLines / 2 + 1);
                    maze.AddEdge(edge);
                }
            }
        }

        public Maze ReadMaze(string fileName)
        {
            var maze = new Maze(LenghtOfMaze);

            using (var reader = new StreamReader(fileName))
            {
                var line = "";
                var countOfLines = 0;
                while((line = reader.ReadLine()) != null && countOfLines / 2 <= LenghtOfMaze)
                {
                    var pattern = countOfLines % 2 == 0 ? new Regex(_mainEvenMazePattern) : new Regex(_mainOddMazePattern);
                    var match = pattern.Match(line);

                    if (match.Success)
                    {
                        if (countOfLines % 2 == 0) AddEvenLine(match.Value, countOfLines, maze);
                        else AddOddLine(match.Value, countOfLines, maze);
                    }
                    else
                    {
                        throw new Exception("Maze in incorrect format");
                    }

                    countOfLines++;
                }
            }

            return maze;
        }
    }
}
