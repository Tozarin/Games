namespace IMDBSolver

open System
open Checker
open IMDBSolver.Loggers
open Repositories
open MoviesReader
open PersonsReader
open TagsReadier
open RatingsReader
open CI

module Main =
    [<EntryPoint>]
    let main args =
        assert( not <| Array.isEmpty args)
        let pathToData = args[0]
        assert(dirExistCheck pathToData)
        assert(filesExistCheck pathToData)

        let moviesRepository : MoviesRepository = Repository()
        let movies = MoviesReader(pathToData, moviesRepository).ReadAndIter()

        let personsRepository : PersonsRepository = Repository()
        ignore <| PersonsReader(pathToData, personsRepository).ReadAndIter()

        let directorsRepositor : DirectorsRepository = Repository()
        let actorsRepository : ActorsRepository = Repository()

        let actorsDirectos = ActorsDirectorsReader(pathToData, actorsRepository, directorsRepositor, personsRepository, moviesRepository).ReadAndIter()

        let moviesIds = MovieIdsMapperReader(pathToData).ReadAndIter()

        let tagsRepository : TagsRepository = Repository()
        ignore <| TagsReader(pathToData, tagsRepository).ReadAndIter()

        let tagToMovies = TagScoresReader(pathToData, moviesIds, tagsRepository, moviesRepository).ReadAndIter()

        ignore <| RatingsReader(pathToData, moviesRepository).ReadAndIter()

        SimpleCI(movies, actorsDirectos, tagToMovies).startLoop()

        0
