namespace Database

open System.Collections.Generic
open Database.Contex
open Database.Model
open Microsoft.EntityFrameworkCore
open Parser.DataClasses
open Parser.Loggers
open Parser.PersonsReader
open Parser.Repositories

module Fillers =

    [<AbstractClass>]
    type Filler<'data, 'dataType>(
        data : 'data,
        connectionFactory : DbFactory,
        logger : ILogger
        ) =

        let mutable saved = 0

        member val data : 'data = data
        member val connectionFactory : DbFactory = connectionFactory
        member val logger : ILogger = logger

        abstract convertData : 'data -> 'dataType seq
        abstract convertToSql : 'dataType -> (string * obj array)

        member self.ConvertAndSave() =
            self.logger.Log("START")

            let converted = self.convertData self.data
            use db = self.connectionFactory.NewConnection()

            for d in converted do
                saved <- saved + 1
                if saved % 250 = 0 then
                    self.logger.Log($"{saved}")

                let query, ps = self.convertToSql d

                try
                    ignore <| db.Database.ExecuteSqlRaw(query, ps)
                with _ ->
                    ()

            self.logger.Log("END")

    type MovieFiller  =
        inherit Filler<MoviesRepository, MMovie>

        new(
            data : MoviesRepository,
            connectionFactory : DbFactory
            ) = {
            inherit Filler<MoviesRepository, MMovie>(data, connectionFactory, SimpleLogger "Movies filler")
            }

        override self.convertData(data : MoviesRepository) =
            seq {
                for m in data.Data do
                    MMovie m
            }

        override self.convertToSql(m : MMovie) =
            let ps = Array.ofList [
                m.Id :> obj;
                m.Name;
                m.AltName;
                m.Score;
            ]

            let q = $"INSERT INTO {movieTableName} VALUES (@p0, @p1, @p2, @p3)"

            (q, ps)


    type PersonFiller  =
        inherit Filler<PersonsRepository, MPerson>

        new(
            data : PersonsRepository,
            connectionFactory : DbFactory
            ) = {
            inherit Filler<PersonsRepository, MPerson>(data, connectionFactory, SimpleLogger "Persons filler")
            }

        override self.convertData(data : PersonsRepository) =
            seq {
                for p in data.Data do
                    MPerson p
            }

        override self.convertToSql(person : MPerson) =
            let ps = Array.ofList [
                person.Id :> obj;
                person.Name;
            ]


            let q = $"INSERT INTO {personTableName} VALUES (@p0, @p1)"

            (q, ps)

    type TagFiller  =
        inherit Filler<TagsRepository, MTag>

        new(
            data : TagsRepository,
            connectionFactory : DbFactory
            ) = {
            inherit Filler<TagsRepository, MTag>(data, connectionFactory, SimpleLogger "Tags filler")
            }

        override self.convertData(data : TagsRepository) =
            seq {
                for t in data.Data do
                    MTag t
            }

        override self.convertToSql(tag : MTag) =
            let ps = Array.ofList [
                tag.Id :> obj;
                tag.Name;
            ]

            let q = $"INSERT INTO {tagTableName} VALUES (@p0, @p1)"

            (q, ps)

    type MADLinkFiller =
        inherit Filler<Dictionary<Person, AsRole list>, MADLink>

        new(
            data : Dictionary<Person, AsRole list>,
            connectionFactory : DbFactory
            ) = {
            inherit Filler<Dictionary<Person, AsRole list>, MADLink>(data, connectionFactory, SimpleLogger "MAD filler")
            }

        override self.convertData(data : Dictionary<Person, AsRole list>) =
            let links = Seq.concat <| seq {
                for kv in data do
                    let p, roles = kv.Key, kv.Value

                    Seq.choose id <| seq {
                        for r in roles do
                            match r with
                            | AsDirector m -> Some (p, m)
                            | _ -> None
                    }
            }

            links |> Seq.distinct |> Seq.map MADLink

        override self.convertToSql(l : MADLink) =
            let ps = Array.ofList [
                l.MovieId :> obj;
                l.PersonId;
            ]

            let q = $"INSERT INTO \"{madLinkTableName}\" VALUES (@p0, @p1)"

            (q, ps)

    type MAALinkFiller =
        inherit Filler<Dictionary<Person, AsRole list>, MAALink>

        new(
            data : Dictionary<Person, AsRole list>,
            connectionFactory : DbFactory
            ) = {
            inherit Filler<Dictionary<Person, AsRole list>, MAALink>(data, connectionFactory, SimpleLogger "MAA filler")
            }

        override self.convertData(data : Dictionary<Person, AsRole list>) =
            let links = Seq.concat <| seq {
                for kv in data do
                    let p, roles = kv.Key, kv.Value

                    Seq.choose id <| seq {
                        for r in roles do
                            match r with
                            | AsActor m -> Some (p, m)
                            | _ -> None
                    }
            }

            links |> Seq.distinct |> Seq.map MAALink

        override self.convertToSql(l : MAALink) =
            let ps = Array.ofList [
                l.MovieId :> obj;
                l.PersonId;
            ]

            let q = $"INSERT INTO \"{maaLinkTableName}\" VALUES (@p0, @p1)"

            (q, ps)

    type TagMovieLinkFiller =
        inherit Filler<Dictionary<Tag, (Movie * float) list>, TagMLink>

        new(
            data : Dictionary<Tag, (Movie * float) list>,
            connectionFactory : DbFactory
            ) = {
            inherit Filler<Dictionary<Tag, (Movie * float) list>, TagMLink>(
                data,
                connectionFactory,
                SimpleLogger "Tag movie link filler"
                )
            }

        override self.convertData(data : Dictionary<Tag, (Movie * float) list>) =
            seq {
                for kv in data do
                    let t, links = kv.Key, kv.Value
                    for m, score in links do
                        TagMLink(score, t, m)
            }

        override self.convertToSql(l : TagMLink) =
            let ps = Array.ofList [
                l.TagId :> obj;
                l.MovieId
                l.Score
            ]

            let q = $"INSERT INTO \"{tagMovieTableName}\" VALUES (@p0, @p1, @p2)"

            (q, ps)
