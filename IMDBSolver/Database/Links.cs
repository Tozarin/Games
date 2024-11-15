using System.ComponentModel.DataAnnotations;

namespace Database;

public class MovieActor
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string MovieId { get; set; }
    public string ActorId { get; set; }

    public MMovie Movie { get; set; } = null!;
    public MPerson Actor { get; set; } = null!;

    public MovieActor(string movieId, string actorId)
    {
        MovieId = movieId;
        ActorId = actorId;
    }
}

public class MovieDirector
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string MovieId { get; set; }
    public string DirectorId { get; set; }

    public MMovie Movie { get; set; } = null!;
    public MPerson Director { get; set; } = null!;

    public MovieDirector(string movieId, string directorId)
    {
        MovieId = movieId;
        DirectorId = directorId;
    }
}

public class MovieTag
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public float Score { get; set; }

    public string? MovieId { get; set; }
    public string? TagId { get; set; }

    public MMovie Movie { get; set; } = null!;
    public MTag Tag { get; set; } = null!;

    public MovieTag(float score, string movieId, string tagId)
    {
        Score = score;
        MovieId = movieId;
        TagId = tagId;
    }
}

public class MovieScore
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public float Score { get; set; }

    public string? ParentMovieId { get; set; }
    public string? ChildeMovieId { get; set; }

    public MMovie ParentMovie { get; set; } = null!;
    public MMovie ChildMovie { get; set; } = null!;

    public MovieScore()
    {

    }

    public MovieScore(float score, string parentId, string childId)
    {
        Score = score;
        ParentMovieId = parentId;
        ChildeMovieId = childId;
    }
}
