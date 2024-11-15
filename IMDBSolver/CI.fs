namespace IMDBSolver

open Parser
open System
open DataClasses
open Operators
open Database
open Queries
open Checker

open System.Collections.Generic

module CI =
    type action =
        | MovieByName of string
        | MoviesByActor of string
        | MoviesByDirector of string
        | MoviesByTag of string
        | LoadByPath of string
        | SimilarMovies of string

    type returnType =
        | MovieOpt of Movie Option
        | Movies of List<Movie>
        | NameAndScore of string * float32
        | NameAndScores of List<string * float32>
        | Error of string
        | Unit

    let (|Movie|Actor|Director|Tag|Load|Top|Other|) = function
        | "m" -> Movie
        | "a" -> Actor
        | "d" -> Director
        | "t" -> Tag
        | "l" -> Load
        | "top" -> Top
        | _ -> Other

    type SimpleCI2(connectionFactory : ContexFactory) =

        member self.getMoviesByName name =
            Movies <| findMoviesByName connectionFactory name

        member self.getMoviesByActor name =
            Movies <| findMoviesByActorName connectionFactory name

        member self.getMoviesByDirector name =
            Movies <| findMoviesByDirectorName connectionFactory name

        member self.getMoviesByTag name =
            Movies <| findMoviesByTagName connectionFactory name

        member self.getSimilarMoviesBy name =
            NameAndScores <| getSimilarMoviesByName connectionFactory name

        member self.Reload path =
            use contex = connectionFactory.NewContex()
            ignore <| contex.Database.EnsureDeleted()
            ignore <| contex.Database.EnsureCreated()

            if dirExistCheck path  && filesExistCheck path then
                let repos = Parser.Main.parse path
                Database.Main.fill connectionFactory repos
                Unit
            else
                Error $"Unable to find neccesaryray files in {path}"

        member self.matchCmd (line : string) =
            let line = Array.map (fun (w : string) -> w.Trim()) <| line.Split(' ')
            if line.Length > 1 then
                Seq.ofArray line[1..] |> String.concat " " |>
                match line[0] with
                | Movie -> fun name -> Some <| MovieByName name
                | Actor -> fun name -> Some <| MoviesByActor name
                | Director -> fun name -> Some <| MoviesByDirector name
                | Tag -> fun name -> Some <| MoviesByTag name
                | Load -> fun path -> Some <| LoadByPath path
                | Top -> fun name -> Some <| SimilarMovies name
                | Other -> fun _ -> None
            else None

        member self.resolveAction = function
            | MovieByName name -> self.getMoviesByName name
            | MoviesByActor name -> self.getMoviesByActor name
            | MoviesByDirector name -> self.getMoviesByDirector name
            | MoviesByTag name -> self.getMoviesByTag name
            | LoadByPath path -> self.Reload path
            | SimilarMovies name -> self.getSimilarMoviesBy name

        member self.resolveRezult = function
            | returnType.MovieOpt movie -> (
                match movie with
                | Some movie -> printfn "\n"; printfn $"{movie}"
                | None -> printfn "\n"; printfn "No such movie"
                    )
            | returnType.Movies ms -> ms.ForEach(fun movie -> printfn "\n"; printfn $"{movie}")
            | returnType.NameAndScore (name, score) -> printfn "\n"; printfn $"| {score} | {name} |"
            | returnType.NameAndScores nss -> printfn "\n"; nss.ForEach (fun (name, score) ->
                printfn $"| {score} | {name} |")
            | returnType.Error msg ->
                Console.ForegroundColor <- ConsoleColor.Red
                Console.WriteLine $"{msg.ToUpper()}"
                Console.ResetColor()
            | returnType.Unit -> ()

        member self.startLoop() =

            printf "~> "
            let line = Console.ReadLine()

            line |> self.matchCmd ><>
                fun cmd -> cmd |> self.resolveAction |> self.resolveRezult
                           Some ()
            |> Option.iter self.startLoop
