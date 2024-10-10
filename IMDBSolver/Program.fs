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

        let sw = DateTime.Now

        let moviesRepository : MoviesRepository = Repository()
        let personsRepository : PersonsRepository = Repository()
        let directorsRepository : DirectorsRepository = Repository()
        let actorsRepository : ActorsRepository = Repository()

        let movies =  MoviesReader(pathToData, moviesRepository).ReadAndIter()
        ignore <| PersonsReader(pathToData, personsRepository).ReadAndIter()

        let tagsRepository : TagsRepository = Repository()

        let actorsDirectors =  ActorsDirectorsReader(pathToData, actorsRepository, directorsRepository, personsRepository, moviesRepository).ReadAndIter()
        let moviesIds = MovieIdsMapperReader(pathToData).ReadAndIter()
        ignore <| TagsReader(pathToData, tagsRepository).ReadAndIter()

        let tagToMovies = TagScoresReader(pathToData, moviesIds, tagsRepository, moviesRepository).ReadAndIter()
        ignore <| RatingsReader(pathToData, moviesRepository).ReadAndIter()

        printfn $"{DateTime.Now - sw}"

        SimpleCI(movies, actorsDirectors, tagToMovies).startLoop()

        0
