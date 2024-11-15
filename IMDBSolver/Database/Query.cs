using System.Diagnostics.CodeAnalysis;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Collections;
using Parser;


namespace Database;

public class Query
{
    private IQueryable<MMovie> findByName(Contex db, string name)
    {

        var movies = db.Movies
                .Where(m => m.Name.Equals(name) || m.AltName.Equals(name))
            .Include(m => m.Actors)
            .Include(m => m.Directors)
            .Include(m => m.Tags)
            .AsSplitQuery();

        return movies;
    }

    private IQueryable<MMovie> findByPartOfName(Contex db, string filter)
    {
        var movies = db.Movies
            .Where(m => m.Name.Contains(filter) || m.AltName.Contains(filter));

        return movies;
    }

    public List<MMovie> FindByName(ContexFactory factory, string name)
    {
        using var db = factory.NewContex();

        return findByName(db, name).ToList();
    }

    public async Task<List<MMovie>> AsyncFindByName(ContexFactory factory, string name)
    {
        await using var db = factory.NewContex();

        var movies = await findByName(db, name).ToListAsync();

        return movies;
    }

    public IQueryable<MMovie> FindByPartOfName(Contex db, string filter)
    {
        var movies = findByPartOfName(db, filter);

        return movies;
    }

    private IQueryable<MMovie> findByTagName(Contex db, string name)
    {

        var movies = db.Movies
            .AsNoTracking()
            .Where(m => m.Tags.Any(t => t.Name == name))
            .Include(m => m.Directors)
            .Include(m => m.Actors)
            .Include(m => m.Tags)
            .AsSplitQuery();

        return movies;
    }

    public List<MMovie> FindByTagName(ContexFactory factory, string name)
    {
        using var db = factory.NewContex();

        return findByTagName(db, name).ToList();
    }

    private IQueryable<MMovie> findByActorName(Contex db, string name)
    {
        var movies = db.Movies
            .AsNoTracking()
            .Where(m => m.Actors.Any(a => a.Name == name))
            .Include(m => m.Directors)
            .Include(m => m.Actors)
            .Include(m => m.Tags)
            .AsSplitQuery();

        return movies;
    }

    public List<MMovie> FindByActorName(ContexFactory factory, string name)
    {
        using var db = factory.NewContex();

        return findByActorName(db, name).ToList();
    }

    private IQueryable<MMovie> findByDirectorName(Contex db, string name)
    {
        var movies = db.Movies
            .AsNoTracking()
            .Where(m => m.Directors.Any(a => a.Name == name))
            .Include(m => m.Directors)
            .Include(m => m.Actors)
            .Include(m => m.Tags)
            .AsSplitQuery();

        return movies;
    }

    public List<MMovie> FindByDirectorName(ContexFactory factory, string name)
    {
        using var db = factory.NewContex();

        return findByDirectorName(db, name).ToList();
    }


    public List<MovieScore> TopSimilarByName(ContexFactory factory, string name)
    {
        using var db = factory.NewContex();

        var blanckMovie = db.Movies
            .Where(m => m.Name == name)
            .Include(m => m.MovieScore)
            .FirstOrDefault();

        if (blanckMovie is null) return new List<MovieScore>();
        if (blanckMovie.IsCashedSimilar) return blanckMovie.MovieScore.OrderByDescending(s => s.Score).ToList();

        var movie = db.Movies
            .Where(m => m.Name == name)
            .AsNoTrackingWithIdentityResolution()
            .Include(m => m.Directors)
                .ThenInclude(d => d.AsDirector)
            .Include(m => m.Actors)
                .ThenInclude(a => a.AsActor)
            .Include(m => m.Tags)
                .ThenInclude(t => t.Movies)
            .AsSplitQuery()
            .First();

        var candidates = movie
            .Directors.SelectMany(d => d.AsDirector)
            .Union(movie.Actors.SelectMany(a => a.AsActor))
            .Union(movie.Tags.SelectMany(t => t.Movies));


        var candidatesQuery = candidates
            .AsQueryable()
            .Include(m => m.Directors)
            .Include(m => m.Actors)
            .Include(m => m.MovieTag)
            .AsSplitQuery();


        var movSim = new MovieSimilarity(10f, 1f, 2f, 1f);

        var scores =
            candidatesQuery.Select(m => movSim.Convert(movie, m));

        var top = scores
            .OrderByDescending(s => s.Score)
            .Where(s => s.ChildeMovieId != name && s.ChildeMovieId != movie.AltName)
            .GroupBy(s => s.Score)
            .Take(10)
            .SelectMany(g => g)
            .ToList();

        var movieToUpdate = db.Movies.First(m => m.Name == name);
        movieToUpdate.IsCashedSimilar = true;
        db.Movies.Update(movieToUpdate);
        db.SaveChanges();

        db.BulkInsert(top);

        return top;
    }

    private IQueryable<MPerson> findPersonByName(Contex db, string name)
    {
        var persons = db.Persons
            .Where(p => p.Name == name);

        return persons;
    }

    public List<MPerson> FindPersonByName(Contex db, string name)
    {
        return findPersonByName(db, name).ToList();
    }

    private IQueryable<MPerson> findPersonByPartOfName(Contex db, string filter)
    {
        var persons = db.Persons
            .Where(m => m.Name.Contains(filter));

        return persons;
    }

    public IQueryable<MPerson> FindPersonByPartOfName(Contex db, string filter)
    {
        var persons = findPersonByPartOfName(db, filter);

        return persons;
    }

    private IQueryable<MTag> findTagByName(Contex db, string name)
    {
        var tags = db.Tags
            .Where(p => p.Name == name);

        return tags;
    }

    public List<MTag> FindTagByName(Contex db, string name)
    {
        return findTagByName(db, name).ToList();
    }

    public IQueryable<MTag> findTagByPartOfName(Contex db, string filter)
    {
        var tags = db.Tags.Where(t => t.Name.Contains(filter));

        return tags;
    }

    public IQueryable<MTag> FindTagByPartOfName(Contex db, string filter)
    {
        var tags = findTagByPartOfName(db, filter);

        return tags;
    }
}
