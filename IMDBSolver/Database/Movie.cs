using System.ComponentModel.DataAnnotations;
using Parser;

namespace Database;

public class MMovie
{
    [Key]
    public string Name { get; set; } = null!;
    public string AltName { get; set; } = null!;
    public float Score { get; set; }

    public bool IsCashedSimilar { get; set; }

    public ICollection<MPerson> Actors { get; set; } = new List<MPerson>();
    public ICollection<MPerson> Directors { get; set; } = new List<MPerson>();
    public ICollection<MTag> Tags { get; set; } = new List<MTag>();
    public ICollection<MMovie> SimilarMovies { get; set; } = new List<MMovie>();
    public ICollection<MMovie> BackSimilarMovies { get; set; } = new List<MMovie>();


    public ICollection<MovieActor> MovieActor { get; set; } = new List<MovieActor>();
    public ICollection<MovieDirector> MovieDirector { get; set; } = new List<MovieDirector>();
    public ICollection<MovieTag> MovieTag { get; set; } = new List<MovieTag>();
    public ICollection<MovieScore> MovieScore { get; set; } = new List<MovieScore>();
    public ICollection<MovieScore> BackMovieScore { get; set; } = new List<MovieScore>();

    public MMovie(string name, string altName, float score)
    {
        Name = name;
        AltName = altName;
        Score = score;
        IsCashedSimilar = false;
    }

    public IEnumerable<MPerson> LoadActors(Contex db)
    {
        db.Entry(this)
            .Collection(m => m.Actors)
            .Load();

        return Actors;
    }

    public IEnumerable<MPerson> LoadDirectors(Contex db)
    {
        db.Entry(this)
            .Collection(m => m.Directors)
            .Load();

        return Directors;
    }

    public IEnumerable<MTag> LoadTags(Contex db)
    {
        db.Entry(this)
            .Collection(m => m.Tags)
            .Load();

        return Tags;
    }
}

public class MovieComparor : IEqualityComparer<MMovie>
{
    public bool Equals(MMovie x, MMovie y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Name == y.Name;
    }

    public int GetHashCode(MMovie obj)
    {
        return obj.Name.GetHashCode();
    }
}

public class MovieSimilarity
{
    private float _dirMul;
    private float _actMul;
    private float _tagMul;
    private float _scoreMul;

    public MovieSimilarity(float dirictorsMul, float actorsMul, float tagsMul, float scoreMul)
    {
        _dirMul = dirictorsMul;
        _actMul = actorsMul;
        _tagMul = tagsMul;
        _scoreMul = scoreMul;
    }

    private int tagMultiplierProgression(float score)
    {
        if (score > 0.9f) return 10;
        if (score > 0.8f) return 5;
        if (score > 0.7f) return 3;
        return 1;
    }

    public MovieScore Convert(MMovie movie, MMovie m)
    {
        var dir = movie.Directors.Any()
            ? movie.Directors.Intersect(m.Directors).Count() / (float)movie.Directors.Count
            : 0.0f;
        var act = movie.Actors.Any()
            ? movie.Actors.Intersect(m.Actors).Count() / (float)movie.Actors.Count
            : 0.0f;
        var tag = movie.MovieTag.Any()
            ? movie.MovieTag.Join(
                      m.MovieTag,
                      l => l.TagId,
                      l => l.TagId,
                      (f, s) => (f.Score + s.Score) * tagMultiplierProgression(f.Score)
                  )
                  .Sum()
              / movie.MovieTag.Count
            : 0.0f;
        var movieScore = m.Score * 0.1f;

        return new MovieScore(
            dir * _dirMul + act * _actMul + tag * _tagMul + movieScore * _scoreMul,
            movie.Name,
            m.Name
        );
    }
}
