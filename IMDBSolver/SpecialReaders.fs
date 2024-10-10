namespace IMDBSolver

open System.Collections.Generic
open FileSignature
open IMDBSolver.DataClasses
open IMDBSolver.Loggers
open Microsoft.FSharp.Core
open Reader
open Repositories
open Operators

open System

module MoviesReader  =

    let fileName = "MovieCodes_IMDB"

    let checkRUName name region =
        if region = "RU" then
            Some name
        else None

    let checkUSName name region =
        if region = "US" || region = "EN" || region = "AU" then
            Some name
        else None

    let regionEq (region : Span<char>) (v : string) =
        MemoryExtensions.Equals(region, v, StringComparison.Ordinal)

    let checkRegion = function
        | "RU" | "US" | "EN" | "AU" -> true
        | _ -> false

    let convertId (id : string) =
        // tt00...0[id]
        Int32.Parse <| id.TrimStart('t')

    type MoviesReader =
        inherit Parser<(int * string) option, Dictionary<string, Movie>>

        val nameToMovie : Dictionary<string, Movie>
        val movies : MoviesRepository
        new(dirPath, movies) = {
            inherit Parser<(int * string) option, Dictionary<string, Movie>>(
                dirPath,
                fileName,
                TSV,
                SimpleLogger "Movies reader"
                )

            nameToMovie = Dictionary<string, Movie>()
            movies = movies
        }

        override self.returnValue() = self.nameToMovie
        override self.split (line : string) =
            let firstIndex = 12 + if line[11] = '\t' then 0 else 1
            let lastIndex = line.IndexOf('\t', firstIndex)
            match line.Substring(lastIndex + 1, 2) with
            | region when checkRegion region ->
                let id = int <| line.Substring(2, 7)
                let name = line.Substring(firstIndex, lastIndex - firstIndex)
                Some (id, name)
            | _ -> None
        override self.preFunction lines =
            ignore <| lines.MoveNext()

        override self.iterFunction splitted =
            match splitted with
            | Some (id, name) ->
                match self.movies.getById id with
                | Some movie ->
                    movie.AddName name
                    self.nameToMovie[name] <- movie
                | None ->
                    let movie = Movie(id, OneLang name)
                    self.movies.put movie
                    self.nameToMovie[name] <- movie
            | None -> ()

module PersonsReader =

    let personsNamesFileName = "ActorsDirectorsNames_IMDB"

    let convertId (id : string) =
       // nm00...0[id]
       assert(id.Length > 2)
       Int32.Parse <| id.Remove(0, 2)

    let convertYear (year : string) =
        let mutable y = 0
        if Int32.TryParse(year, &y) then Some y
        else None

    type PersonsReader =
        inherit Parser<int * string, bool>

        val personsRepository : PersonsRepository
        new(dirPath, personsRepository) = {
            inherit Parser<int * string, bool>(
                dirPath,
                personsNamesFileName,
                TXT,
                SimpleLogger "Persons reader"
                )

            personsRepository = personsRepository
        }

        override self.returnValue() = true
        override self.split (line : string) =
            let id = int <| line.Substring(2, 7)
            let name =
                let lastIndex = line.IndexOf('\t', 10)
                line.Substring(10, lastIndex - 10)
            ( id, name )
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            self.personsRepository.put <| Person splitted

    let (|Actor|Director|Other|) = function
        | 'a' | 's' -> Actor
        | 'd' -> Director
        | _ -> Other

    let isActorDirector = function
        | Actor | Director -> true | _ -> false

    let personsCodesFileName = "ActorsDirectorsCodes_IMDB"

    type ActorsDirectorsReader =
        inherit Parser<(int * int * bool) option, Dictionary<string, Movie list>>

        val personsToMovie : Dictionary<string, Movie list>
        val actors : ActorsRepository
        val directors : DirectorsRepository
        val persons : PersonsRepository
        val movies : MoviesRepository

        new(dirPath, actors, directors, persons, movies) = {
            inherit Parser<(int * int * bool) option, Dictionary<string, Movie list>>(
                dirPath,
                personsCodesFileName,
                TSV,
                SimpleLogger "Actors Directors reader"
                )

            personsToMovie = Dictionary<string, Movie list>()
            actors = actors
            directors = directors
            persons = persons
            movies = movies
        }

        override self.returnValue() = self.personsToMovie
        override self.split (line : string) =
            let startOfPerson = if line[11] = '\t' then 14 else 15
            let startOfRole = startOfPerson + 8

            let role = line[startOfRole]

            match role with
            | Actor ->
                let movieId = int <| line.Substring(2, 7)
                let personId = int <| line.Substring(startOfPerson, 7)
                Some ( movieId, personId, true )
            | Director ->
                let movieId = int <| line.Substring(2, 7)
                let personId = int <| line.Substring(startOfPerson, 7)
                Some ( movieId, personId, false )

            | _ -> None


        override self.preFunction _ = ()
        override self.iterFunction splitted =
            match splitted with
            | Some (movieId, personId, isActor) ->
                let person = self.persons.getById personId
                match person with
                | Some person ->
                    let movie = self.movies.getById movieId
                    match movie with
                    | Some movie ->
                        let name = person.FullName
                        self.personsToMovie[name] <-
                            if self.personsToMovie.ContainsKey name then movie :: self.personsToMovie[name]
                            else [movie]

                        if isActor then
                           let actor = self.actors.getOrPut <| Actor(person)
                           self.movies.change movie (fun (movie : Movie) -> movie.Actors <- Set.add actor movie.Actors)
                        else
                           let director = self.directors.getOrPut <| Director(person)
                           self.movies.change movie (fun (movie : Movie) -> movie.Director <- Some director)
                    | None -> ()

                | _ -> ()

            | None -> ()

            // splitted ><> fun (movieId, personId, isActor) ->
            //
            //     self.persons.getById personId ><> fun person ->
            //         self.movies.getById movieId ><> fun movie ->
            //             let name = person.FullName
            //             self.personsToMovie[name] <-
            //                 if self.personsToMovie.ContainsKey name then movie :: self.personsToMovie[name]
            //                 else [movie]
            //
            //             if isActor then
            //                 let actor = self.actors.getOrPut <| Actor(person)
            //                 self.movies.change movie (fun (movie : Movie) -> movie.Actors <- Set.add actor movie.Actors)
            //             else
            //                 let director = self.directors.getOrPut <| Director(person)
            //                 self.movies.change movie (fun (movie : Movie) -> movie.Director <- Some director)
            //             Some ()
            // |> ignore


module TagsReadier =

    let idsMapperFileName = "links_IMDB_MovieLens"

    let convertId (word : string) =
        let mutable id = 0
        if Int32.TryParse(word, &id) then Some id
        else None

    type MovieIdsMapperReader =
        inherit Parser<int * int, Dictionary<int, int>>

        val mapper : Dictionary<int, int>

        new(dirPath : string) = {
            inherit Parser<int * int, Dictionary<int, int>>(
                dirPath,
                idsMapperFileName,
                CSV,
                SimpleLogger "Tags reader"
                )

            mapper = Dictionary<int, int>()
        }

        override self.returnValue() = self.mapper
        override self.split (line : string) =
            let indexOfComma = line.IndexOf(',')
            let id = int32 <| line.Substring(0, indexOfComma)
            let imdbId = int32 <| line.Substring(1 + indexOfComma, 7)
            (imdbId, id)
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            let imdbId, tmdbId = splitted
            self.mapper[tmdbId] <- imdbId

    let tagCodesFileName = "TagCodes_MovieLens"

    type TagsReader =
        inherit Parser<int * string, bool>

        val tagsRepository : TagsRepository

        new(dirPath : string, tagsRepository : TagsRepository) = {
            inherit Parser<int * string, bool>(dirPath, tagCodesFileName, CSV, SimpleLogger "Tag Codes reader")
            tagsRepository = tagsRepository
        }

        override self.returnValue() = true
        override self.split (line : string) =
            let offset = line.IndexOf(',')
            let id = int <| line.Substring(0, offset)
            let name = line.Substring(offset + 1)
            ( id, name )
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            self.tagsRepository.put <| Tag splitted

    let tagScoresFileName = "TagScores_MovieLens"

    type TagScoresReader =
        inherit Parser<(int * int * float) option, Dictionary<string, Movie list>>

        val tagToMovies : Dictionary<string, Movie list>

        val movieMapper : Dictionary<int, int>
        val tagsRepository : TagsRepository
        val moviesRepository : MoviesRepository

        new(
            dirPath : string,
            movieMapper : Dictionary<int, int>,
            tagsRepository : TagsRepository,
            moviesRepository : MoviesRepository
            ) = {
                inherit Parser<(int * int * float) option, Dictionary<string, Movie list>>(
                    dirPath,
                    tagScoresFileName,
                    CSV,
                    SimpleLogger "Tags Scores reader"
                    )

                tagToMovies = Dictionary<string, Movie list>()
                movieMapper = movieMapper
                tagsRepository = tagsRepository
                moviesRepository = moviesRepository
            }

        override self.returnValue() = self.tagToMovies

        override self.split (line : string) =
            let offsetTag = line.IndexOf(',')
            let offsetScore = line.IndexOf(',', offsetTag + 1)

            let score = float <| line.Substring(offsetScore + 1)

            if score >= 0.5 then
                let id = int32 <| line.Substring(0, offsetTag)
                let tag = int32 <| line.Substring(offsetTag + 1, offsetScore - offsetTag - 1)
                Some (id, tag, score)
            else
                None


        override self.preFunction _ = ()
        override self.iterFunction splitted =
            match splitted with
            | Some (movieId, tagId, score) ->
                let isGet, imdbId = self.movieMapper.TryGetValue(movieId)
                if isGet then

                    let tag = self.tagsRepository.getById tagId
                    match tag with
                    | Some tag ->

                        let movie = self.moviesRepository.getById imdbId
                        match movie with
                        | Some movie ->
                            let tagName = tag.Name
                            self.tagToMovies[tagName] <-
                                if self.tagToMovies.ContainsKey tagName then movie :: self.tagToMovies[tagName]
                                else [movie]

                            self.moviesRepository.change movie (fun (movie : Movie) -> movie.Tags <- Set.add (tag, score) movie.Tags)
                        | None -> ()

                    | _ -> ()
                else
                    ()
            | None -> ()


            (*splitted ><> fun (movieId, tagId, score) ->
                let isGet, imdbId = self.movieMapper.TryGetValue(movieId)

                if isGet then Some () else None ><> fun _ ->
                    self.tagsRepository.getById tagId ><> fun tag ->
                        self.moviesRepository.getById imdbId ><> fun movie ->
                            let tagName = tag.Name
                            self.tagToMovies[tagName] <-
                                if self.tagToMovies.ContainsKey tagName then movie :: self.tagToMovies[tagName]
                                else [movie]

                            self.moviesRepository.change movie (fun (movie : Movie) -> movie.Tags <- Set.add (tag, score) movie.Tags)
                            Some ()

            |> ignore*)


module RatingsReader =

    let ratingsFileName = "Ratings_IMDB"

    type RatingsReader =
        inherit Parser<int * float, bool>

        val moviesRepository : MoviesRepository
        new(dirPath : string, moviesRepository : MoviesRepository) = {
            inherit Parser<int * float, bool>(dirPath, ratingsFileName, TSV, SimpleLogger "Ratings reader")
            moviesRepository = moviesRepository
        }

        override self.returnValue() = true
        override self.split (line : string) =
            let id = int32 <| line.Substring(2, 7)
            let score = float <| line.Substring(10, 4)
            (id, score)
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            let id, rating = splitted
            let movie = self.moviesRepository.getById id

            Option.iter (
                fun movie ->
                    self.moviesRepository.change movie
                        (fun (movie : Movie) -> movie.Score <- Some rating)
                )
                movie
