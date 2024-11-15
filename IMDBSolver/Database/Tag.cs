using System.ComponentModel.DataAnnotations;

namespace Database;

public class MTag
{
    [Key]
    public string Name { get; set; } = null!;

    public ICollection<MMovie> Movies { get; set; } = new List<MMovie>();

    public ICollection<MovieTag> TagMovie { get; set; } = new List<MovieTag>();

    public MTag(string name)
    {
        Name = name;
    }

    public IEnumerable<MMovie> LoadMovies(Contex db)
    {
        db.Entry(this)
            .Collection(t => t.Movies)
            .Load();

        return Movies;
    }
}
