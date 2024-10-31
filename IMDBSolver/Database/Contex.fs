namespace Database

open Database.Model
open Microsoft.EntityFrameworkCore

module Contex =

    let movieTableName = "movie"
    let personTableName = "person"
    let tagTableName = "tag"
    let madLinkTableName = "movieDirector"
    let maaLinkTableName = "movieActor"
    let tagMovieTableName = "MovieTag"

    type Db(connectionString : string) =
        inherit DbContext()

        member val ConnectionString : string = connectionString

        [<DefaultValue>]
        val mutable persons : DbSet<MPerson>
        [<DefaultValue>]
        val mutable tags : DbSet<MTag>
        [<DefaultValue>]
        val mutable movies : DbSet<MMovie>

        [<DefaultValue>]
        val mutable moviesActors : DbSet<MAALink>
        [<DefaultValue>]
        val mutable moviesDirectors : DbSet<MADLink>
        [<DefaultValue>]
        val mutable moviesTags : DbSet<TagMLink>

        member public this.Persons
            with get() = this.persons
            and set ps = this.persons <- ps
        member public this.Tags
            with get() = this.tags
            and set ts = this.tags <- ts
        member public this.Movies
            with get() = this.movies
            and set ms = this.movies <- ms

        member public this.MoviesActors
            with get() = this.moviesActors
            and set ms = this.moviesActors <- ms
        member public this.MoviesDirectors
            with get() = this.moviesDirectors
            and set ms = this.moviesDirectors <- ms
        member public this.MoviesTags
            with get() = this.moviesTags
            and set ms = this.moviesTags <- ms

        override _.OnConfiguring(optionsBuilder : DbContextOptionsBuilder) =
            ignore <| optionsBuilder.EnableSensitiveDataLogging(true)
            ignore <| optionsBuilder.UseNpgsql(connectionString)

        override _.OnModelCreating(modelBuilder  : ModelBuilder) =

            // Person
            let personEntity = modelBuilder.Entity<MPerson>()
            ignore <| personEntity.ToTable(personTableName)
            ignore <| personEntity.Property(fun p -> p.Id).HasColumnName("Id")
            ignore <| personEntity.Property(fun p -> p.Name).HasColumnName("Name")
            // ignore <| personEntity.Ignore("MoviesAsActors")
            // ignore <| personEntity.Ignore("MoviesAsDirectors")
            ignore <| personEntity.HasKey("Id")

            // Movie
            let movieEntity = modelBuilder.Entity<MMovie>()
            ignore <| movieEntity.ToTable(movieTableName)
            ignore <| movieEntity.Property(fun t -> t.Id).HasColumnName("Id")
            ignore <| movieEntity.Property(fun t -> t.Name).HasColumnName("Name")
            ignore <| movieEntity.Property(fun t -> t.AltName).HasColumnName("AltName")
            ignore <| movieEntity.Property(fun t -> t.Score).HasColumnName("Score")
            ignore <| movieEntity.HasKey("Name")

            // Tag
            let tagEntity = modelBuilder.Entity<MTag>()
            ignore <| tagEntity.ToTable(tagTableName)
            ignore <| tagEntity.Property(fun t -> t.Id).HasColumnName("Id")
            ignore <| tagEntity.Property(fun t -> t.Name).HasColumnName("Name")
            ignore <| tagEntity.HasKey("Name")


            // Links
            let linkTable = modelBuilder.Entity<MAALink>()
            ignore <| linkTable.ToTable(maaLinkTableName)
            ignore <| linkTable.HasKey("PersonId", "MovieId")
            ignore <| linkTable.HasOne("Person").WithMany("MoviesAsActorsLink").HasForeignKey("PersonId")
            ignore <| linkTable.HasOne("Movie").WithMany("ActorsLink").HasForeignKey("MovieId")

            let linkTable = modelBuilder.Entity<MADLink>()
            ignore <| linkTable.ToTable(madLinkTableName)
            ignore <| linkTable.HasKey("PersonId", "MovieId")
            ignore <| linkTable.HasOne("Person").WithMany("MoviesAsDirectorsLink").HasForeignKey("PersonId")
            ignore <| linkTable.HasOne("Movie").WithMany("DirectorsLink").HasForeignKey("MovieId")

            let linkTable = modelBuilder.Entity<TagMLink>()
            ignore <| linkTable.ToTable(tagMovieTableName)
            ignore <| linkTable.HasKey("TagId", "MovieId")
            ignore <| linkTable.HasOne("Tag").WithMany("MoviesLink").HasForeignKey("TagId")
            ignore <| linkTable.HasOne("Movie").WithMany("TagsLink").HasForeignKey("MovieId")


    type DbFactory(connectionString : string) =
        member self.NewConnection() = new Db(connectionString)
        member self.NewConnectionWithoutTracking() =
            let db = new Db(connectionString)
            db.ChangeTracker.AutoDetectChangesEnabled <- false

            db
