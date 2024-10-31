namespace Parser

open MoviesReader
open RatingsReader
open TagsReadier
open PersonsReader
open Repositories

module Main =

    let parse pathToData =
        let moviesRepository : MoviesRepository = Repository()
        let personsRepository : PersonsRepository = Repository()
        let tagsRepository : TagsRepository = Repository()
        let directorsRepository : DirectorsRepository = Repository()
        let actorsRepository : ActorsRepository = Repository()

        let nameToMovies = MoviesReader(pathToData, moviesRepository).ReadAndIter()
        ignore <| PersonsReader(pathToData, personsRepository).ReadAndIter()


        let actorsDirectors = ActorsDirectorsReader(pathToData, actorsRepository, directorsRepository, personsRepository, moviesRepository).ReadAndIter()
        let moviesIds = MovieIdsMapperReader(pathToData).ReadAndIter()
        ignore <| TagsReader(pathToData, tagsRepository).ReadAndIter()

        let tagToMovies = TagScoresReader(pathToData, moviesIds, tagsRepository, moviesRepository).ReadAndIter()
        ignore <| RatingsReader(pathToData, moviesRepository).ReadAndIter()

        (moviesRepository, nameToMovies, personsRepository, actorsDirectors, tagsRepository, tagToMovies)

    let main args =
        0
