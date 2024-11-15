namespace Database

open System
open System.Collections.Generic
open Microsoft.EntityFrameworkCore
open Microsoft.FSharp.Control
open System.Linq
open Parser.DataClasses

module Queries =

    let makeMovie (m : MMovie) =

        let name = if m.AltName.Equals("") then OneLang(m.Name) else BiLang(m.Name, m.AltName)

        let tags = Set <| seq {
            for i, t in Seq.indexed m.Tags do
                let score = t.TagMovie.FirstOrDefault(fun tag -> tag.MovieId.Equals name)
                let score = if isNull score then -1. else float score.Score
                Tag(i, t.Name, score)
        }

        let actors = Set <| seq {
            for i, p in Seq.indexed m.Actors do
                Actor <| Person(i, p.Name)
        }

        let directors = Set <| seq {
            for i, p in Seq.indexed m.Directors do
                Director <| Person(i, p.Name)
        }

        let score = if float m.Score = -1. then None else m.Score |> float |> Some

        Movie(-1, name, actors, tags, directors, score)

    let findMoviesByName (connectionFactory : ContexFactory) (name : string) =

        let movies = Query().FindByName(connectionFactory, name)

        let movies = Seq.map makeMovie movies

        List<Movie>(movies)

    let findPersonByName (db : Contex) (name : string) =

        let persons = Query().FindPersonByName(db, name)

        List<MPerson>(persons)


    let asyncFindMoviesByName (connectionFactory : ContexFactory) (name : string) =
        Query().AsyncFindByName(connectionFactory, name)

    let findByPartOfName (db : Contex) (filter : string) =
        Query().FindByPartOfName(db, filter)

    let findPersonByPartOfName (db : Contex) (filter : string) =
        Query().FindPersonByPartOfName(db, filter)

    let findTagByName (db : Contex) (name : string) =
        Query().FindTagByName(db, name)

    let findTagByPartOfName (db : Contex) (filter : string) =
        Query().FindTagByPartOfName(db, filter)

    let findMoviesByActorName (connectionFactory : ContexFactory) (name : string) =
        let movies = Query().FindByActorName(connectionFactory, name)

        let movies = Seq.map makeMovie movies

        List<Movie>(movies)

    let findMoviesByDirectorName (connectionFactory : ContexFactory) (name : string) =
        let movies = Query().FindByDirectorName(connectionFactory, name)

        let movies = Seq.map makeMovie movies

        List<Movie>(movies)


    let findMoviesByTagName (connectionFactory : ContexFactory) (name : string) =
        let movies = Query().FindByTagName(connectionFactory, name)

        let movies = Seq.map makeMovie movies

        List<Movie>(movies)

    let getSimilarMoviesByName (connectionFactory : ContexFactory) (name : string) =

        let top = Query().TopSimilarByName(connectionFactory, name)

        List<string * float32>(seq { for m in top do m.ChildeMovieId, m.Score })
