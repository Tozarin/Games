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

		public (int, int) IsConnectedWithOn(Edge edge)
        {
			foreach (var e in _edges)
				if (e.IsConnectedWith(edge))
					return e.GetSamePoint(edge);

			return (-1, -1);
        }

		public List<List<Edge>> FindTwoPaths((int, int) firstPoint, (int, int) secondPoint)
		{
			var firstPath = new List<Edge>();
			var secondPath = new List<Edge>();

			var prevIndex = _indexMap[firstPoint];
			var indexFrom = -1;
			var secondIndex = _indexMap[secondPoint];
			var map = _indexMap.Keys.ToArray();
			var second = false;

			while (true)
			{
				for (var i = 0; i < _size; i++)
				{
					if (_vertexies[prevIndex, i] && indexFrom != i)
					{
						if (!second)
							firstPath.Add(new Edge(map[prevIndex], map[i]));
						else
							secondPath.Add(new Edge(map[prevIndex], map[i]));

						indexFrom = prevIndex;
						prevIndex = i;

						if (i == secondIndex)
							second = true;

						break;
					}
				}

				if (second && prevIndex == _indexMap[firstPoint])
					break;
			}

			return new List<List<Edge>> { firstPath, secondPath };
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
				var added = false;

				foreach (var inner in inners)
					if (inner.Any((x) => x.IsConnectedWith(edge)))
					{
						inner.Add(edge);
						added = true;
					}

				if (!added)
					inners.Add(new List<Edge> { edge });
			}

			return (cycle, inners);
        }
	}
}
