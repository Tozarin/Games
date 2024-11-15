using System.ComponentModel.DataAnnotations;

namespace Database;

public class MPerson
{
    [Key]
    public string Name { get; set; } = null!;

    public ICollection<MMovie> AsActor { get; set; } = new List<MMovie>();
    public ICollection<MMovie> AsDirector { get; set; } = new List<MMovie>();

    public ICollection<MovieActor> ActorMovie { get; set; } = new List<MovieActor>();
    public ICollection<MovieDirector> DirectorMovie { get; set; } = new List<MovieDirector>();

    public MPerson(string name)
    {
        Name = name;
    }

    public IEnumerable<MMovie> LoadAsActor(Contex db)
    {
        db.Entry(this)
            .Collection(p => p.AsActor)
            .Load();

        return AsActor;
    }

    public IEnumerable<MMovie> LoadAsDirector(Contex db)
    {
        db.Entry(this)
            .Collection(p => p.AsDirector)
            .Load();

        return AsDirector;
    }
}
