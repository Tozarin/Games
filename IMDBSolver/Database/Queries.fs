namespace Database

open System
open System.Collections.Generic
open Contex
open Database.Model
open Microsoft.EntityFrameworkCore
open Microsoft.FSharp.Control
open System.Linq
open Parser.DataClasses

module Queries =

    let makeMovie (connectionFactory : DbFactory) (m : MMovie) =

        use db = connectionFactory.NewConnection()

        let name = if m.AltName.Equals("") then OneLang(m.Name) else BiLang(m.Name, m.AltName)

        let tags = db.Tags.Where(fun t -> t.MoviesLink.Any(fun l -> l.MovieId = m.Id)).ToList()
        let tags = Set <| seq {
            for t in tags do
                Tag(t.Id, t.Name), -1.
        }

        let actors = db.Persons.Where(fun p -> p.MoviesAsActorsLink.Any(fun l -> l.MovieId = m.Id)).ToList()
        let actors = Set <| seq { for p in actors do Actor <| Person(p.Id, p.Name) }

        let directors = db.Persons.Where(fun p -> p.MoviesAsDirectorsLink.Any(fun l -> l.MovieId = m.Id)).ToList()
        let dir =
            if directors.Count = 0 then None
            else
                directors.First() |> fun d -> Person(d.Id, d.Name) |> Director |> Some

        let score = if m.Score < 0. then None else Some m.Score

        Movie(m.Id, name, actors, tags, dir, score)

    let findMoviesByName (connectionFactory : DbFactory) (name : string) =

        use db = connectionFactory.NewConnection()

        let movies = db.Movies.Where(fun m -> m.Name.Equals name || m.AltName.Equals name).ToList()

        List<Movie> (seq { for m in movies do makeMovie connectionFactory m })


    let findMoviesByActorName (connectionFactory : DbFactory) (name : string) =

        use db = connectionFactory.NewConnection()
        let persons = db.Persons.Where(fun p -> p.Name.Equals name).Select(_.Id)

        let links = db.MoviesActors.Where(fun l -> persons.Contains l.PersonId).Select(_.MovieId)

        let movies = db.Movies.Where(fun m -> links.Contains m.Id).ToList()

        List<Movie>(seq { for m in movies do makeMovie connectionFactory m })

    let findMoviesByDirectorName (connectionFactory : DbFactory) (name : string) =

        use db = connectionFactory.NewConnection()
        let persons = db.Persons.Where(fun p -> p.Name.Equals name).Select(_.Id)

        let links = db.MoviesDirectors.Where(fun l -> persons.Contains l.PersonId).Select(_.MovieId)

        let movies = db.Movies.Where(fun m -> links.Contains m.Id).ToList()

        List<Movie>(seq { for m in movies do makeMovie connectionFactory m })


    let findMoviesByTagName (connectionFactory : DbFactory) (name : string) =

        use db = connectionFactory.NewConnection()
        let tags = db.Tags.Where(fun t -> t.Name.Equals name).Select(_.Id)

        let links = db.MoviesTags.Where(fun l -> tags.Contains l.TagId).Select(_.MovieId)

        let movies = db.Movies.Where(fun m -> links.Contains m.Id).ToList()

        List<Movie>(seq { for m in movies do makeMovie connectionFactory m })
