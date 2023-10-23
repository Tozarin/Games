namespace MazeEscape
{
    public class FiguresManager
    {
        private List<Figure> _figures;

        public FiguresManager()
        {
            _figures = new List<Figure>();
        }

        public void AddEdge(Edge edge)
        {
            Figure havePoint = null;
            Figure toRemove = null;

            foreach(var figure in _figures)
            {
                if (figure.ContainsFullEdge(edge)) break;

                if (figure.ContainsPointFromEdge(edge))
                {
                    if (havePoint == null) 
                    { 
                        havePoint = figure; 
                    }
                    else
                    {
                        havePoint.Union(figure, edge);
                        toRemove = figure;
                        break;
                    }
                }
            }

            if (toRemove != null)
            {
                _figures.Remove(toRemove);
                return;
            }

            if (havePoint != null)
            {
                havePoint.AddEdge(edge);
            }
            else
            {
                var newFigure = new Figure();
                newFigure.AddEdge(edge);
                _figures.Add(newFigure);
            }
        }

        public bool ContainsPoint((float, float) point) => _figures.Any((x) => x.ContainsPoint(point));
    }
}
