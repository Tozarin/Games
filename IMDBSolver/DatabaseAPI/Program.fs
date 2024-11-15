namespace Database

open System
open System.Collections.Generic
open EFCore.BulkExtensions
open Parser.DataClasses
open Parser.Loggers
open Parser.PersonsReader
open Parser.Repositories

module Main =

    let logger = SimpleLogger("Loader") :> ILogger

    type role =
        | Director of MovieDirector
        | Actor of MovieActor

    let fillMovies (db : Contex) (nameToMovies : Dictionary<string, Movie>) =
        logger.Log("START Movies")

        let movies = seq {
            for m in nameToMovies.Values do
                let name, altName = m.GetNames
                let score = m.NonOptionScore |> float32
                MMovie(name, altName, score)
        }

        logger.Log("START Movies load")

        db.BulkInsert(movies)

        logger.Log("END Movies")

    let fillTags (db : Contex) (tags : TagsRepository) =
        logger.Log("START Tags")

        let tags = seq {
            for t in tags.Data do
                MTag t.Name
        }

        logger.Log("START Tags load")

        db.BulkInsert(tags)

        logger.Log("END Tags")

    let fillPersons (db : Contex) (persons : PersonsRepository) =
        logger.Log("START Persons")

        let persons = Seq.distinct <| seq {
            for p in persons.Data do
                p.FullName
        }

        let persons = Seq.map MPerson persons

        logger.Log("START Persons load")

        db.BulkInsert(persons)

        logger.Log("END Persons")

    let fillPersonsLinks (db : Contex) (actorsDirectors : Dictionary<string, AsRole list>) =
        logger.Log("START Persons Links")

        let personLinks = seq {
            for kv in actorsDirectors do
                let p, links = kv.Key, kv.Value

                for l in links do
                    match l with
                    | AsActor m ->
                        let name, altName =  m.GetNames
                        Actor <| MovieActor(name, p)
                        if not <| altName.Equals "" then
                            Actor <| MovieActor(altName, p)

                    | AsDirector m ->
                        let name, altName = m.GetNames
                        Director <| MovieDirector(name, p)
                        if not <| altName.Equals "" then
                            Director <| MovieDirector(altName, p)
        }

        let actorsLinks = Seq.choose (function | Actor m -> Some m | _ -> None) personLinks
        let directorsLinks = Seq.choose (function | Director m -> Some m | _ -> None) personLinks

        logger.Log("START Actors Links load")
        db.BulkInsert(actorsLinks)

        logger.Log("START Directors Links load")
        db.BulkInsert(directorsLinks)

        logger.Log("END Persons Links")

    let fillTagsLinks (db : Contex) (tagToMovies : Dictionary<string, (Movie * float) list>) =
        logger.Log("START Tags links")

        let movieTag = seq {
            for kv in tagToMovies do
                let t, links = kv.Key, kv.Value
                for m, s in links do
                    let score = float32 s
                    let name, altName = m.GetNames
                    MovieTag(score, name, t)
                    if not <| altName.Equals "" then
                        MovieTag(score, altName, t)
        }

        logger.Log("START Tags links load")

        db.BulkInsert(movieTag)

        logger.Log("END Tags links")

    let fillScores (db : Contex) (scores : Dictionary<string, (float * Movie) seq>) =
        logger.Log("START Scores")

        let scores = seq {
            for kv in scores do
                let m, links = kv.Key, kv.Value
                for score, child in links do
                    let score = float32 score
                    let parentName = m
                    let childName, childAltName = child.GetNames

                    MovieScore(score, parentName, childName)

                    if not <| childAltName.Equals "" then
                        MovieScore(score, parentName, childAltName)

        }

        logger.Log("START Scores load")

        scores |> Seq.splitInto 7000 |> Seq.iteri
                                             (fun i ss ->
            db.BulkInsert ss
            printfn $"Saved {i} of 7000 chanks")

        logger.Log("END Scores load")

    let fill (connectionFactory : ContexFactory)
        (
            movies : MoviesRepository,
            nameToMovies : Dictionary<string, Movie>,
            persons : PersonsRepository,
            actorsDirectors : Dictionary<string, AsRole list>,
            tags : TagsRepository,
            tagToMovies : Dictionary<string, (Movie * float) list>
            ) =

        use db = connectionFactory.NewContex()

        db.ChangeTracker.AutoDetectChangesEnabled <- false

        fillMovies db nameToMovies
        fillTags db tags
        fillPersons db persons
        fillPersonsLinks db actorsDirectors
        fillTagsLinks db tagToMovies

        db.ChangeTracker.AutoDetectChangesEnabled <- true

    [<EntryPoint>]
    let main args =
        0
