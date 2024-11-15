using Microsoft.EntityFrameworkCore;

namespace Database;

public class Contex : DbContext
{
    public DbSet<MMovie> Movies { get; set; }
    public DbSet<MPerson> Persons { get; set; }
    public DbSet<MTag> Tags { get; set; }
    public DbSet<MovieActor> MovieActor { get; set; }
    public DbSet<MovieDirector> MovieDirector { get; set; }
    public DbSet<MovieTag> MovieTag { get; set; }
    public DbSet<MovieScore> MovieScore { get; set; }

    public string ConnectionString { get; }

    public Contex(string connectionString)
    {
        ConnectionString = connectionString;

        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(ConnectionString);
        builder.EnableSensitiveDataLogging();
        builder.EnableDetailedErrors();
        //builder.LogTo(Console.Write);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<MMovie>()
            .HasMany(m => m.Actors)
            .WithMany(p => p.AsActor)
            .UsingEntity<MovieActor>(
                l =>
                    l.HasOne<MPerson>(l => l.Actor)
                        .WithMany(l => l.ActorMovie)
                        .HasForeignKey(l => l.ActorId),
                l =>
                    l.HasOne<MMovie>(l => l.Movie)
                        .WithMany(l => l.MovieActor)
                        .HasForeignKey(l => l.MovieId)
            );

        builder.Entity<MMovie>()
            .HasMany(m => m.Directors)
            .WithMany(p => p.AsDirector)
            .UsingEntity<MovieDirector>(
                l =>
                    l.HasOne<MPerson>(l => l.Director)
                        .WithMany(p => p.DirectorMovie)
                        .HasForeignKey(l => l.DirectorId),
                l =>
                    l.HasOne<MMovie>(l => l.Movie)
                        .WithMany(m => m.MovieDirector)
                        .HasForeignKey(l => l.MovieId)
            );

        builder.Entity<MMovie>()
            .HasMany(m => m.Tags)
            .WithMany(t => t.Movies)
            .UsingEntity<MovieTag>(
                l =>
                    l.HasOne<MTag>(l => l.Tag)
                        .WithMany(t => t.TagMovie)
                        .HasForeignKey(l => l.TagId),
                l =>
                    l.HasOne<MMovie>(l => l.Movie)
                        .WithMany(m => m.MovieTag)
                        .HasForeignKey(l => l.MovieId)
            );

        builder.Entity<MMovie>()
            .HasMany(m => m.SimilarMovies)
            .WithMany(m => m.BackSimilarMovies)
            .UsingEntity<MovieScore>(
                l =>
                    l.HasOne<MMovie>(l => l.ChildMovie)
                        .WithMany(m => m.BackMovieScore)
                        .HasForeignKey(l => l.ChildeMovieId),
                l =>
                    l.HasOne<MMovie>(l => l.ParentMovie)
                        .WithMany(m => m.MovieScore)
                        .HasForeignKey(l => l.ParentMovieId)
            );
    }
}

public class ContexFactory
{
    public string ConnectionString { get; }

    public ContexFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public ContexFactory()
    {
        ConnectionString =
            $"Host=localhost;Port=5432;Database=imdb;Username=postgres;Password=admin;Timeout=600;CommandTimeout=600;Include Error Detail=true;";
    }

    public Contex NewContex()
    {
        return new Contex(ConnectionString);
    }
}
