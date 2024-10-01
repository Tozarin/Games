namespace IMDBSolver

open System.Collections.Generic
open FileSignature
open IMDBSolver.DataClasses
open IMDBSolver.Loggers
open Microsoft.FSharp.Core
open Reader
open Repositories
open MyConcurrentDictionary

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

    let convertId (id : string) =
        // tt00...0[id]
        Int32.Parse <| id.TrimStart('t')

    type MoviesReader =
        inherit Parser<int * string * string, Dictionary<string, Movie>>

        val mutable currentId : int
        val mutable currentRUName : string option
        val mutable currentUSName : string option

        val nameToMovie : Dictionary<string, Movie>
        val movies : MoviesRepository
        new(dirPath, movies) = {
            inherit Parser<int * string * string, Dictionary<string, Movie>>(
                dirPath,
                fileName,
                TSV,
                SimpleLogger("Movies reader")
                )

            nameToMovie = Dictionary<string, Movie>()
            movies = movies
            currentId = -1
            currentRUName = None
            currentUSName = None
        }

        override self.returnValue() = self.nameToMovie
        override self.split (line : ReadOnlySpan<char>) =
            let id = Int32.Parse(line.Slice(2, 7))
            let firstIndex = 12 + if line[11] = '\t' then 0 else 1
            let lastIndex = MemoryExtensions.IndexOf(line.Slice firstIndex, '\t')
            let name = line.Slice(firstIndex, lastIndex).ToString()
            let region = line.Slice(firstIndex + lastIndex + 1, 2).ToString()
            (id, name, region)
        override self.preFunction lines =
            ignore <| lines.MoveNext()
            let id, name, region = self.split lines.Current
            self.currentId <- id
            self.currentRUName <- checkRUName name region
            self.currentUSName <- checkUSName name region

        override self.iterFunction splitted =
            let id, name, region = splitted

            if not (id = self.currentId) then
                match self.currentRUName, self.currentUSName with
                    | Some ru, Some us ->
                        let movie = Movie(self.currentId, BiLang (ru, us))
                        self.movies.put movie
                        self.nameToMovie[ru] <- movie
                        self.nameToMovie[us] <- movie

                    | Some name, _ ->
                        let movie = Movie(self.currentId, RU name)
                        self.movies.put movie
                        self.nameToMovie[name] <- movie

                    | _, Some name ->
                        let movie = Movie(self.currentId, US name)
                        self.movies.put movie
                        self.nameToMovie[name] <- movie

                    | _ -> ()

                self.currentId <- id
                self.currentRUName <- None
                self.currentUSName <- None

            if Option.isNone self.currentRUName then
               self.currentRUName <- checkRUName name region
            if Option.isNone self.currentUSName then
               self.currentUSName <- checkUSName name region

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
        inherit Parser<int * string * int option * int option, bool>

        val personsRepository : PersonsRepository
        new(dirPath, personsRepository) = {
            inherit Parser<int * string * int option * int option, bool>(
                dirPath,
                personsNamesFileName,
                TXT,
                SimpleLogger("Persons reader")
                )

            personsRepository = personsRepository
        }

        override self.returnValue() = true
        override self.split (line : ReadOnlySpan<char>) =
            let id = Int32.Parse(line.Slice(2, 7))
            let name =
                let lastIndex = MemoryExtensions.IndexOf(line.Slice 10, '\t')
                line.Slice(10, lastIndex).ToString()
            ( id, name, None, None )
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            self.personsRepository.put <| Person splitted

    let (|Actor|Director|Other|) = function
        | "actor" | "actress" | "self" -> Actor
        | "director" -> Director
        | _ -> Other

    let isActorDirector = function
        | Actor | Director -> true | _ -> false

    let personsCodesFileName = "ActorsDirectorsCodes_IMDB"

    type ActorsDirectorsReader =
        inherit Parser<int * int * string, Dictionary<string, Movie list>>

        val personsToMovie : Dictionary<string, Movie list>
        val actors : ActorsRepository
        val directors : DirectorsRepository
        val persons : PersonsRepository
        val movies : MoviesRepository

        new(dirPath, actors, directors, persons, movies) = {
            inherit Parser<int * int * string, Dictionary<string, Movie list>>(
                dirPath,
                personsCodesFileName,
                TSV,
                SimpleLogger("Actors-Directors reader")
                )

            personsToMovie = Dictionary<string, Movie list>()
            actors = actors
            directors = directors
            persons = persons
            movies = movies
        }

        override self.returnValue() = self.personsToMovie
        override self.split (line : ReadOnlySpan<char>) =
            let movieId = Int32.Parse(line.Slice(2, 7))
            let startOfPerson = if line[11] = '\t' then 14 else 15
            let personId = Int32.Parse(line.Slice(startOfPerson, 7))
            let startOfRole = startOfPerson + 8
            let endOfRole = MemoryExtensions.IndexOf(line.Slice startOfRole, '\t')
            let role = line.Slice(startOfRole, endOfRole).ToString()
            ( movieId, personId, role )
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            let movieId, personId, category = splitted
            let person = self.persons.getById personId
            let movie = self.movies.getById movieId

            match person, movie with
            | Some person, Some movie ->

                if isActorDirector category then
                    let name = person.FullName
                    self.personsToMovie[name] <-
                        if self.personsToMovie.ContainsKey name then movie :: self.personsToMovie[name]
                        else [movie]

                match category with
                | Actor ->
                    let actor = self.actors.getOrPut <| Actor(person)
                    self.movies.change movie (fun (movie : Movie) -> movie.Actors <- Set.add actor movie.Actors)
                | Director ->
                    let director = self.directors.getOrPut <| Director(person)
                    self.movies.change movie (fun (movie : Movie) -> movie.Director <- Some director)
                | Other -> ()

            | _ -> ()

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
                SimpleLogger("Movie ids mapper reader")
                )

            mapper = Dictionary<int, int>()
        }

        override self.returnValue() = self.mapper
        override self.split (line : ReadOnlySpan<char>) =
            let indexOfComma = line.IndexOf(',')
            let id = Int32.Parse(line.Slice(0, indexOfComma))
            let imdbId = Int32.Parse(line.Slice(1 + indexOfComma, 7))
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
            inherit Parser<int * string, bool>(dirPath, tagCodesFileName, CSV, SimpleLogger("Tags reader"))
            tagsRepository = tagsRepository
        }

        override self.returnValue() = true
        override self.split (line : ReadOnlySpan<char>) =
            let offset = line.IndexOf(',')
            let id = Int32.Parse(line.Slice(0, offset))
            let name = line.Slice(offset + 1).ToString()
            ( id, name )
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            self.tagsRepository.put <| Tag splitted

    let tagScoresFileName = "TagScores_MovieLens"

    type TagScoresReader =
        inherit Parser<int * int * float, Dictionary<string, Movie list>>

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
                inherit Parser<int * int * float, Dictionary<string, Movie list>>(
                    dirPath,
                    tagScoresFileName,
                    CSV,
                    SimpleLogger("Tag scores reader")
                    )

                tagToMovies = Dictionary<string, Movie list>()
                movieMapper = movieMapper
                tagsRepository = tagsRepository
                moviesRepository = moviesRepository
            }

        override self.returnValue() = self.tagToMovies
        override self.split (line : ReadOnlySpan<char>) =
            let offsetTag = line.IndexOf(',')
            let offsetScore =  MemoryExtensions.IndexOf(line.Slice(offsetTag + 1), ',')

            let id = Int32.Parse(line.Slice(0, offsetTag))
            let tag = Int32.Parse(line.Slice(offsetTag + 1, offsetScore))

            let scoreIndex = offsetTag + offsetScore + 2
            let scoreSlice = min 5 <| line.Length - scoreIndex
            let score = float(line.Slice(scoreIndex, scoreSlice).ToString())
            (id, tag, score)
        override self.preFunction _ = ()
        override self.iterFunction splitted =
            let movieId, tagId, score = splitted

            if score >= 0.5 then
                let mutable imdbId = -1
                ignore <| self.movieMapper.TryGetValue(movieId, &imdbId)

                let tag = self.tagsRepository.getById tagId
                let movie = self.moviesRepository.getById imdbId

                match tag, movie with
                | Some tag, Some movie ->
                    let tagName = tag.Name
                    self.tagToMovies[tagName] <-
                        if self.tagToMovies.ContainsKey tagName then movie :: self.tagToMovies[tagName]
                        else [movie]

                    self.moviesRepository.change movie (fun (movie : Movie) -> movie.Tags <- Set.add (tag, score) movie.Tags)
                | _ -> ()

module RatingsReader =

    let ratingsFileName = "Ratings_IMDB"

    type RatingsReader =
        inherit Parser<int * float, bool>

        val moviesRepository : MoviesRepository
        new(dirPath : string, moviesRepository : MoviesRepository) = {
            inherit Parser<int * float, bool>(dirPath, ratingsFileName, TSV, SimpleLogger("Raitings reader"))
            moviesRepository = moviesRepository
        }

        override self.returnValue() = true
        override self.split (line : ReadOnlySpan<char>) =
            let id = Int32.Parse(line.Slice(2, 7))
            let score = float(line.Slice(10, 4).ToString())
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
