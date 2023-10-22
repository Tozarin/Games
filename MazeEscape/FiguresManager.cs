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
                        havePoint.Union(edge, figure);
                        toRemove = figure;
                        break;
                    }
                }
            }

            if (toRemove != null)
            {
                _figures.Remove(toRemove);
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
    }
}
