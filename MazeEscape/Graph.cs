namespace MazeEscape
{
	public class Graph
	{

		private int _size;
		private bool[,] _vertexies;
		private Dictionary<(int, int), int> _indexMap;
		private List<Edge> _edges;

		public Graph()
		{
			
		}

		public void FromEdges(List<Edge> edges)
        {
			_indexMap = new Dictionary<(int, int), int>();

			int count = 0;
			foreach (var edge in edges)
            {
				if (!_indexMap.ContainsKey(edge.FirstPoint))
					_indexMap[edge.FirstPoint] = count++;

				if (!_indexMap.ContainsKey(edge.SecondPoint))
					_indexMap[edge.SecondPoint] = count++;
            }

			_size = count;

			_vertexies = new bool[_size, _size];
			foreach (var edge in edges)
            {
				_vertexies[_indexMap[edge.FirstPoint], _indexMap[edge.SecondPoint]] = true;
				_vertexies[_indexMap[edge.SecondPoint], _indexMap[edge.FirstPoint]] = true;
			}

			_edges = edges;
		}

		public (List<Edge>, List<List<Edge>>) GetCycleAndInners()
        {
			var stack = new List<int>();
			var visited = new bool[_size];

			int dfs(int vertex, int vertexFrom)
            {
				if (visited[vertex]) return vertex;

				stack.Add(vertex);
				visited[vertex] = true;

				for (var i = 0; i < _size; i++)
                {
					if (_vertexies[vertex, i] && i != vertexFrom)
                    {
						var rez = dfs(i, vertex);

						if (rez < 0) continue;

						return rez;
                    }
				}

				stack.Remove(vertex);
				return -1;
            }

			var vertex = dfs(0, -1);

			var indexCycle = new List<int>();
			var cycleFlag = false;
			foreach (var v in stack)
            {
				if (v == vertex)
					cycleFlag = true;

				if (cycleFlag) indexCycle.Add(v);
            }

			var map = _indexMap.Keys.ToArray();
			var cycle = new List<Edge>();

			var prevIndex = indexCycle.Last();
			for (var i = 0; i < indexCycle.Count; i++)
            {
				cycle.Add(new Edge(map[prevIndex], map[indexCycle[i]]));
				prevIndex = indexCycle[i];
            }

			var inners = new List<List<Edge>>();
			var otherEdges = new List<Edge>(_edges);
			otherEdges.RemoveAll((x) => cycle.Any((y) => y.IsSameAs(x)));
			foreach (var edge in otherEdges)
            {
				for (var i = 0; i < inners.Count; i++)
                {
					if (inners[i].First().FirstPoint == edge.SecondPoint)
                    {
						inners[i] = new List<Edge> { edge }.Concat(inners[i]).ToList();
                    }

					if (inners[i].Last().SecondPoint == edge.FirstPoint)
                    {
						inners[i].Add(edge);
                    }
                }

				inners.Add(new List<Edge> { edge });
            }

			return (cycle, inners);
        }
	}
}
