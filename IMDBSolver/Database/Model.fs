namespace Database

open System.Collections.Generic
open Parser.DataClasses

module Model =

    type MPerson(id : int, name : string) =

        member val Id : int = id with get, set
        member val Name : string = name with get, set

        member val MoviesAsActorsLink : MAALink ICollection = ResizeArray [] with get, set
        member val MoviesAsDirectorsLink : MADLink ICollection = ResizeArray [] with get, set

        new() = MPerson(-1, "")
        new(person : Person) = MPerson(person.Id, person.FullName)

    and MTag(id : int, name : string) =

        member val Id : int = id with get, set
        member val Name : string = name with get, set

        member val MoviesLink : TagMLink ICollection = ResizeArray [] with get, set

        new() = MTag(-1, "")
        new(tag : Tag) = MTag(tag.Id, tag.Name)

    and MMovie(id : int, name : string, altName : string, score : float) =

        member val Id : int = id with get, set
        member val Name : string = name with get, set
        member val AltName : string = altName with get, set
        member val Score : float = score with get, set

        member val ActorsLink : MAALink ICollection = ResizeArray [] with get, set
        member val DirectorsLink : MADLink ICollection = ResizeArray [] with get, set
        member val TagsLink : TagMLink ICollection = ResizeArray [] with get, set

        new() = MMovie(-1, "", "", -1.)
        new(movie : Movie) =
            let name, altName =
                match movie.Name with
                | OneLang name -> name, ""
                | BiLang (fst, snd) -> fst, snd

            let score = match movie.Score with | Some score -> score | None -> -1.

            MMovie(movie.Id, name, altName, score)

    and MAALink(personId : int, person : MPerson, movieId : int, movie : MMovie) =

        member val PersonId : int = personId with get, set
        member val Person : MPerson = person with get, set

        member val MovieId : int = movieId with get, set
        member val Movie : MMovie = movie with get, set

        new() = MAALink(-1, MPerson(), -1, MMovie())
        new(person : MPerson, movie : MMovie) = MAALink(person.Id, person, movie.Id, movie)
        new(person : Person, movie : Movie) = MAALink(MPerson person, MMovie movie)

    and MADLink(personId : int, person : MPerson, movieId : int, movie : MMovie) =

        member val PersonId : int = personId with get, set
        member val Person : MPerson = person with get, set

        member val MovieId : int = movieId with get, set
        member val Movie : MMovie = movie with get, set

        new() = MADLink(-1, MPerson(), -1, MMovie())
        new(person : MPerson, movie : MMovie) = MADLink(person.Id, person, movie.Id, movie)
        new(person : Person, movie : Movie) = MADLink(MPerson person, MMovie movie)

    and TagMLink(score : float, tagId : int, tag : MTag, movieId : int, movie : MMovie) =

        member val Score : float = score with get, set

        member val TagId : int = tagId with get, set
        member val Tag : MTag = tag with get, set

        member val MovieId : int = movieId with get, set
        member val Movie : MMovie = movie with get, set

        new() = TagMLink(-1., -1, MTag(), -1, MMovie())
        new(score : float, tag : MTag, movie : MMovie) = TagMLink(score, tag.Id, tag, movie.Id, movie)
        new(score : float, tag : Tag, movie : Movie) = TagMLink(score, MTag tag, MMovie movie)
