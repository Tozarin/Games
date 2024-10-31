namespace Database

open System
open System.Collections.Generic
open Contex
open Database.Contex
open Database.Fillers
open Database.Model
open Microsoft.EntityFrameworkCore
open EFCore.BulkExtensions
open Parser.DataClasses
open Parser.PersonsReader
open Parser.Repositories
open Model
open System.Linq

module Main =

    let fillMovies (connectionFactory : DbFactory) (movies : MoviesRepository) =


        let movies = seq {
            for m in movies.Data do
                MMovie m
        }

        // db.Movies.AddRange(movies)
        // ignore <| db.SaveChanges()

        use db = connectionFactory.NewConnection()
        db.BulkInsert(movies)

        printfn "I saved all movies"

    let fillTags (connectionFactory : DbFactory) (tags : TagsRepository) =

        let tags = seq {
            for t in tags.Data do
                MTag t
        }

        // db.Tags.AddRange(tags)
        // ignore <| db.SaveChanges()
        use db = connectionFactory.NewConnection()
        db.BulkInsert(tags, fun o -> o.SetOutputIdentity <- true)

        printfn "I saved all tags"

    let fillPersons (connectionFactory : DbFactory) (persons : PersonsRepository) =
        let mutable count = 0

        let persons = seq {
            for p in persons.Data do
                count <- count + 1
                if count % 100 = 0 then
                    printfn $"I saved {count} persons"

                MPerson p
        }

        // db.Persons.AddRange(persons)
        // ignore <| db.SaveChanges()

        use db = connectionFactory.NewConnection()
        db.BulkInsert(persons)

        printfn "I saved all persons"

    type MAsRole =
        | MAsActor of MAALink
        | MAsDirector of MADLink
        | Skip

    let fillDirectorsActorsLinks (db : Db) (actorsDirectors : Dictionary<Person, AsRole list>) =
        let mutable count = 0

        printfn "start fill actorsdirectorslink"

        let links = Seq.concat <| seq {
            for kv in actorsDirectors do
                let p, roles = kv.Key, kv.Value
                seq {
                    for r in roles do
                        p, r
                }
        }

        printfn "start collect actorsdirectorslink"

        let links = Seq.distinct links

        let links = seq {
            for p, r in links do
                count <- count + 1
                if count % 100 = 0 then
                    printfn $"I saved {count} personlinks"

                match r with
                    | AsActor m ->
                        let p = db.Persons.Single(fun person -> p.Id = person.Id)
                        let m = db.Movies.Single(fun movie -> m.Id = movie.Id)
                        MAsActor <| MAALink(p, m)
                    | AsDirector m ->
                        let p = db.Persons.Single(fun person -> p.Id = person.Id)
                        let m = db.Movies.Single(fun movie -> m.Id = movie.Id)
                        MAsDirector <| MADLink(p, m)
        }

        let asActors = links |> Seq.choose (function | MAsActor link -> Some link | _ -> None)
        db.MoviesActors.AddRange(asActors)
        ignore <| db.SaveChanges()
        printfn "I saved all actors"

        let asDirectors = links |> Seq.choose (function | MAsDirector link -> Some link | _ -> None)
        db.MoviesDirectors.AddRange(asDirectors)
        ignore <| db.SaveChanges()
        printfn "I saved all directors"

    let fillTagsMoviesLinks
        (connectionFactory : DbFactory)
        (tagToMovies : Dictionary<Tag, (Movie * float) list>)
        (moviesId : Dictionary<int, Movie>) =

        use db = connectionFactory.NewConnection()

        let links = seq {
            for kv in tagToMovies do
                let t, links = kv.Key, kv.Value
                for m, score in links do

                    // let t = db.Tags.Single(fun tag -> tag.Id = t.Id)
                    // let m = db.Movies.Single(fun movie -> movie.Id = m.Id)

                    TagMLink(score, t, m)
        }

        // db.MoviesTags.AddRange(links)
        // ignore <| db.SaveChanges()

        use db = connectionFactory.NewConnection()
        db.BulkInsert(links)

        printfn $"I saved all tag links"

    let fill (connectionFactory : DbFactory)
        (
            movies : MoviesRepository,
            nameToMovies : Dictionary<int, Movie>,
            persons : PersonsRepository,
            actorsDirectors : Dictionary<Person, AsRole list>,
            tags : TagsRepository,
            tagToMovies : Dictionary<Tag, (Movie * float) list>
            ) =

        // db.ChangeTracker.AutoDetectChangesEnabled <- false
        // db.ChangeTracker.AutoDetectChangesEnabled <- true
        fillMovies connectionFactory movies
        fillTags connectionFactory tags
        fillTagsMoviesLinks connectionFactory tagToMovies nameToMovies
        // fillPersons connectionFactory persons
        // fillDirectorsActorsLinks connectionFactory actorsDirectors

        // use db = connectionFactory.NewConnection()
        // db.ChangeTracker.AutoDetectChangesEnabled <- false
        //
        // //MovieFiller(movies, connectionFactory).ConvertAndSave()
        // //PersonFiller(persons, connectionFactory).ConvertAndSave()
        // //TagFiller(tags, connectionFactory).ConvertAndSave()
        // MADLinkFiller(actorsDirectors, connectionFactory).ConvertAndSave()
        // MADLinkFiller(actorsDirectors, connectionFactory).ConvertAndSave()
        // TagMovieLinkFiller(tagToMovies, connectionFactory).ConvertAndSave()
        //
        // db.ChangeTracker.AutoDetectChangesEnabled <- true

    [<EntryPoint>]
    let main args =
        0
