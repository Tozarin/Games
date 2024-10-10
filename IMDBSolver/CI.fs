namespace IMDBSolver

open System
open DataClasses
open Operators

open System.Collections.Generic

module CI =
    type action =
        | MovieByName of string
        | MoviesByPerson of string
        | MoviesByTag of string

    let (|Movie|Person|Tag|Other|) = function
        | "m" -> Movie
        | "p" -> Person
        | "t" -> Tag
        | _ -> Other

    type SimpleCI(
        movies : Dictionary<string, Movie>,
        actorsDirectors : Dictionary<string, Movie list>,
        tags : Dictionary<string, Movie list>
        ) =

        let movies = movies
        let actorsDirectors = actorsDirectors
        let tags = tags

        member self.getMoviesByName name =
            match movies.TryGetValue name with
            | true, movie -> [movie]
            | _ -> []

        member self.getMoviesByPerson name =
            match actorsDirectors.TryGetValue name with
            | true, movies -> movies
            | _ -> []

        member self.getMoviesByTag name =
            match tags.TryGetValue name with
            | true, movies -> movies
            | _ -> []

        member self.matchCmd (line : string) =
            let line = Array.map (fun (w : string) -> w.Trim()) <| line.Split(' ')
            if line.Length > 1 then
                Seq.ofArray line[1..] |> String.concat " " |>
                match line[0] with
                | Movie -> fun name -> Some <| MovieByName name
                | Person -> fun name -> Some <| MoviesByPerson name
                | Tag -> fun name -> Some <| MoviesByTag name
                | Other -> fun _ -> None
            else None

        member self.resolveAction = function
            | MovieByName name -> self.getMoviesByName name
            | MoviesByPerson name -> self.getMoviesByPerson name
            | MoviesByTag name -> self.getMoviesByTag name

        member self.startLoop() =

            printf "> "
            let line = Console.ReadLine()

            line |> self.matchCmd ><>
                fun cmd -> List.iter (fun movie -> printfn "\n\n"; printfn $"{movie}")
                           <| self.resolveAction cmd
                           Some ()
            |> Option.iter self.startLoop
