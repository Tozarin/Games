namespace MazeEscape
{
    public class Figure
    {
        private List<Cycle> _cycles;
        private Dictionary<Cycle, List<List<Edge>>> _inners;

        public List<Cycle> Cycles => new List<Cycle>(_cycles);
        public Dictionary<Cycle, List<List<Edge>>> Inners => new Dictionary<Cycle, List<List<Edge>>>(_inners);

        public Figure()
        {
            _cycles = new List<Cycle>();
            _inners = new Dictionary<Cycle, List<List<Edge>>>();
        }

        public bool ContainsPoint((float, float) point) => _cycles.Any((x) => x.ContainsPoint(point));

        public void AddEdge(Edge edge)
        {
            if (ContainsFullEdge(edge)) return;

            var connectedInners = new List<(Cycle, List<Edge>)>();
            var connectedCycles = new List<Cycle>();

            foreach (var cycle in _cycles)
                if (cycle.ContainsPointFromEdge(edge)) connectedCycles.Add(cycle);

            foreach (var (cycle, inners) in _inners)
                foreach (var inner in inners)
                    if (inner.Any((x) => x.IsConnectedWith(edge)))
                        connectedInners.Add((cycle, inner));

            var countOfInners = connectedInners.Count;
            var countOfCycles = connectedCycles.Count;

            if ((countOfInners, countOfCycles) == (0, 0))
            {
                // new figure 
                _inners.Add(new Cycle(), new List<List<Edge>> { new List<Edge> { edge } });
            }
            else if ((countOfInners, countOfCycles) == (1, 0))
            {
                var connectedFirst = false;
                var connectedSecond = false;
                var cycle = false;
                foreach (var e in connectedInners.First().Item2)
                {
                    if (e.Points.Contains(edge.FirstPoint))
                        connectedFirst = true;
                    if (e.Points.Contains(edge.SecondPoint))
                        connectedSecond = true;

                    cycle = connectedFirst && connectedSecond;
                    if (cycle) break;
                }

                if (cycle)
                {
                    connectedInners.First().Item2.Add(edge);

                    var newCycle = new Cycle();
                    var newInners = newCycle.FromSingleInner(connectedInners.First().Item2);

                    _inners.Remove(connectedInners.First().Item1);
                    _inners.Add(newCycle, newInners);
                    _cycles.Add(newCycle);

                    foreach (var inner in newInners)
                    {
                        if (inner.Any((x) => connectedInners.First().Item1.ContainsPointFromEdge(x)))
                        {
                            _inners[connectedInners.First().Item1] = _inners[connectedInners.First().Item1]
                                .Select(
                                    (x) => { if (x == connectedInners.First().Item2) return inner; return x; }
                                ).ToList();
                        }
                    }

                    return;
                }

                _inners[connectedInners.First().Item1] = _inners[connectedInners.First().Item1]
                    .Select(
                        (x) => { if (x == connectedInners.First().Item2) x.Add(edge); return x; }
                    ).ToList();
            }
            else if ((countOfInners, countOfCycles) == (0, 1))
            {
                if (_inners.ContainsKey(connectedCycles.First()))
                    _inners[connectedCycles.First()].Add(new List<Edge> { edge });
                else
                    _inners.Add(connectedCycles.First(), new List<List<Edge>> { new List<Edge> { edge } });
            }
            else
            {
                if (((countOfInners, countOfCycles) == (2, 0) && connectedInners.First().Item1 == connectedInners.Last().Item1)
                    || ((countOfInners, countOfCycles) == (1, 1) && connectedInners.First().Item1 == connectedCycles.First()))
                {
                    var rootCycle = connectedInners.First().Item1;
                    var oldInners = _inners[rootCycle]
                        .Where(
                            (x) => x != connectedInners.First().Item2 && x != connectedInners.Last().Item2
                        ).ToList();

                    _inners.Remove(rootCycle);
                    // same inners in variant of 1 1 ?
                    var newInners = rootCycle.ExpandCycle(connectedInners.First().Item2, connectedInners.Last().Item2, edge);

                    _inners.Add(rootCycle, oldInners.Concat(newInners).ToList());
                }
                else
                {
                    Cycle firstCycle = null;
                    Cycle secondCycle = null;
                    List<Edge> newInner = new List<Edge>();

                    if ((countOfInners, countOfCycles) == (2, 0))
                    {
                        firstCycle = connectedInners.First().Item1;
                        secondCycle = connectedInners.Last().Item1;

                        newInner = new List<Edge>()
                            .Concat(connectedInners.First().Item2)
                            .Concat(connectedInners.Last().Item2)
                            .ToList();
                        newInner.Add(edge);
                    }
                    else if ((countOfInners, countOfCycles) == (0, 2))
                    {
                        firstCycle = connectedCycles.First();
                        secondCycle = connectedCycles.Last();

                        newInner.Add(edge);
                    }
                    else if ((countOfInners, countOfCycles) == (1, 1))
                    {
                        firstCycle = connectedCycles.First();
                        secondCycle = connectedInners.First().Item1;

                        newInner = new List<Edge>()
                            .Concat(connectedInners.First().Item2)
                            .ToList();
                        newInner.Add(edge);
                    }

                    List<Edge> connectedInner = null;
                    foreach (var inner in _inners[firstCycle])
                        if (_inners[secondCycle].Contains(inner))
                        {
                            connectedInner = inner;
                            break;
                        }

                    if (connectedInner != null)
                    {
                        List<Edge> firstInner = null;
                        List<Edge> secondInner = null;
                        if ((countOfInners, countOfCycles) == (2, 0))
                        {
                            firstInner = connectedInners.First().Item2;
                            secondInner = connectedInners.Last().Item2;
                        }
                        else if ((countOfInners, countOfCycles) == (0, 2))
                        {

                        }
                        else if ((countOfInners, countOfCycles) == (1, 1))
                        {
                            secondInner = connectedInners.First().Item2;
                        }

                        var oldFristInners = _inners[firstCycle].Where((x) => x != connectedInner && x != firstInner).ToList();
                        var oldSecondInners = _inners[secondCycle].Where((x) => x != connectedInner && x != secondInner).ToList();
                        var newInners = oldFristInners.Concat(oldSecondInners).ToList();

                        _inners.Remove(firstCycle);
                        _inners.Remove(secondCycle);
                        _cycles.Remove(firstCycle);
                        _cycles.Remove(secondCycle);

                        var newCycle = firstCycle.FromTwoCyclesAndInners(secondCycle, newInner, connectedInner);
                        _inners.Add(newCycle, newInners);
                        _cycles.Add(newCycle);
                    }
                    else
                    {
                        if ((countOfInners, countOfCycles) == (2, 0))
                        {
                            _inners[firstCycle] = _inners[firstCycle]
                                .Select(
                                    (x) => { if (x == connectedInners.First().Item2) return newInner; return x; }
                                ).ToList();
                            _inners[secondCycle] = _inners[secondCycle]
                                .Select(
                                    (x) => { if (x == connectedInners.Last().Item2) return newInner; return x; }
                                ).ToList();
                        }
                        else if ((countOfInners, countOfCycles) == (0, 2))
                        {
                            _inners[firstCycle].Add(newInner);
                            _inners[secondCycle].Add(newInner);
                        }
                        else if ((countOfInners, countOfCycles) == (1, 1))
                        {
                            _inners[firstCycle].Add(newInner);
                            _inners[secondCycle] = _inners[secondCycle]
                                .Select(
                                    (x) => { if (x == connectedInners.Last().Item2) return newInner; return x; }
                                ).ToList();
                        }
                    }
                }
            }
        }

        public bool ContainsPointFromEdge(Edge edge)
        {
            return _cycles.Any((x) => x.ContainsPointFromEdge(edge))
                || _inners.Any((x) => { return x.Value.Any((y) => y.Any((z) => z.IsConnectedWith(edge))); });
        }

        public bool ContainsFullEdge(Edge edge)
            => _cycles.Any((x) => x.ContainsFullEdge(edge));

        public void Union(Figure figure, Edge edge)
        {
            var newCycles = new List<Cycle>();
            var newInners = new Dictionary<Cycle, List<List<Edge>>>();

            var connectedInners = new List<(Cycle, List<Edge>)>();
            var connectedCycles = new List<Cycle>();

            foreach (var cycle in _cycles)
            {
                if (cycle.ContainsPointFromEdge(edge))
                    connectedCycles.Add(cycle);
                newCycles.Add(cycle);
            }

            foreach (var cycle in figure.Cycles)
            {
                if (cycle.ContainsPointFromEdge(edge))
                    connectedCycles.Add(cycle);

                newCycles.Add(cycle);
            }

            foreach (var (cycle, inners) in _inners)
            {
                foreach (var inner in inners)
                {
                    if (inner.Any((x) => x.IsConnectedWith(edge)))
                        connectedInners.Add((cycle, inner));
                }

                newInners.Add(cycle, inners);
            }

            foreach (var (cycle, inners) in figure.Inners)
            {
                foreach (var inner in inners)
                {
                    if (inner.Any((x) => x.IsConnectedWith(edge)))
                        connectedInners.Add((cycle, inner));
                }

                newInners.Add(cycle, inners);
            }

            var countOfInners = connectedInners.Count;
            var countOfCycles = connectedCycles.Count;

            if ((countOfInners, countOfCycles) == (2, 0))
            {
                var firstCycle = connectedInners.First().Item1;
                var secondCycle = connectedInners.Last().Item1;
                var firstInner = connectedInners.First().Item2;
                var secondInner = connectedInners.Last().Item2;

                var newInner = firstInner.Concat(secondInner).ToList();
                newInner.Add(edge);

                newInners[firstCycle] = newInners[firstCycle]
                    .Select(
                        (x) => { if (x == firstInner) return newInner; return x; }
                    ).ToList();
                newInners[secondCycle] = newInners[secondCycle]
                    .Select(
                        (x) => { if (x == secondInner) return newInner; return x; }
                    ).ToList();
            }
            else if ((countOfInners, countOfCycles) == (0, 2))
            {
                newInners[connectedCycles.First()].Add(new List<Edge> { edge });
                newInners[connectedCycles.Last()].Add(new List<Edge> { edge });
            }
            else if ((countOfInners, countOfCycles) == (1, 1))
            {
                var newInner = connectedInners.First().Item2;
                newInner.Add(edge);

                newInners[connectedCycles.First()].Add(newInner);
                newInners[connectedInners.First().Item1] = newInners[connectedInners.First().Item1]
                    .Select(
                        (x) => { if (x == connectedInners.First().Item2) return newInner; return x; }
                    ).ToList();
            }
            else
            {
                throw new Exception("Missing invariant at union");
            }

            _cycles = newCycles;
            _inners = newInners;

            foreach (var cycle in _inners.Keys)
                if (cycle.Lenght == 0)
                {
                    _inners.Remove(cycle);
                    break;
                }
        }
    }
}
