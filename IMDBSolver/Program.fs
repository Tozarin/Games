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

        let moviesPersonsTask = async {
            let! movies = Async.StartChild <| async { return MoviesReader(pathToData, moviesRepository).ReadAndIter() }
            let! _ = Async.StartChild <| async { return PersonsReader(pathToData, personsRepository).ReadAndIter() }

            let! rz = movies
            return rz
        }

        let movies = Async.RunSynchronously moviesPersonsTask

        let tagsRepository : TagsRepository = Repository()

        let actDirMovIdsTagsTask = async {
            let! actorsDirectors = Async.StartChild <| async { return ActorsDirectorsReader(pathToData, actorsRepository, directorsRepository, personsRepository, moviesRepository).ReadAndIter() }
            let! moviesIds = Async.StartChild <| async { return MovieIdsMapperReader(pathToData).ReadAndIter() }
            let! _ = Async.StartChild <| async { return TagsReader(pathToData, tagsRepository).ReadAndIter() }

            let! adrz = actorsDirectors
            let! mirz = moviesIds

            return (adrz, mirz)
        }

        let actorsDirectors, moviesIds = Async.RunSynchronously actDirMovIdsTagsTask

        let tagsRaitingsTask = async {
            let! tagToMovies = Async.StartChild <| async { return TagScoresReader(pathToData, moviesIds, tagsRepository, moviesRepository).ReadAndIter() }
            let! _ = Async.StartChild <| async { return RatingsReader(pathToData, moviesRepository).ReadAndIter() }

            let! rz = tagToMovies
            return rz
        }

        let tagToMovies = Async.RunSynchronously tagsRaitingsTask

        printfn $"{DateTime.Now - sw}"

        SimpleCI(movies, actorsDirectors, tagToMovies).startLoop()

        0
