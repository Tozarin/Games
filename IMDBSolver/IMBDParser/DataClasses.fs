namespace Parser

open System
open System.Diagnostics

module DataClasses =
    type Person(id : int, fullName : string) =
        let id = id
        let fullName = fullName

        member self.Id = id
        member self.FullName = fullName

        interface IComparable<Person> with
            override self.CompareTo obj = compare self.Id obj.Id
        interface IComparable with
            override self.CompareTo obj =
                match obj with
                | null -> 1
                | :? Person as obj -> (self :> IComparable<_>).CompareTo obj
                | _ -> 1

        override self.Equals obj =
            (=) 0 <| (self :> IComparable).CompareTo obj
        override self.GetHashCode() = id.GetHashCode()
        override self.ToString() = $"{id}. {fullName}"

    type Actor(person : Person) =
        let person = person

        member self.Person = person
        member self.FullName = person.FullName

        interface IComparable<Actor> with
            override self.CompareTo obj = compare self.Person obj.Person
        interface IComparable with
            override self.CompareTo obj =
                match obj with
                | null -> 1
                | :? Actor as obj -> (self :> IComparable<_>).CompareTo obj
                | _ -> 1

        override self.Equals obj =
            (=) 0 <| (self :> IComparable).CompareTo obj
        override self.GetHashCode() = person.GetHashCode()
        override self.ToString() = $"Actor {person}"

    type Director(person : Person) =
        let person = person

        member self.Person = person
        member self.FullName = person.FullName

        interface IComparable<Director> with
            override self.CompareTo obj = compare self.Person obj.Person
        interface IComparable with
            override self.CompareTo obj =
                match obj with
                | null -> 1
                | :? Director as obj -> (self :> IComparable<_>).CompareTo obj
                | _ -> 1

        override self.Equals obj =
            (=) 0 <| (self :> IComparable).CompareTo obj
        override self.GetHashCode() = person.GetHashCode()
        override self.ToString() = $"Director {person}"

    type Tag(id : int, name : string, score : float) =
        let id = id
        let name = name
        let score = score

        new(t : Tag, score : float) = Tag(t.Id, t.Name, score)

        member self.Id = id
        member self.Name = name
        member self.Score = score

        interface IComparable<Tag> with
            override self.CompareTo obj = compare self.Id obj.Id
        interface IComparable with
            override self.CompareTo obj =
                match obj with
                | null -> 1
                | :? Tag as obj -> (self :> IComparable<_>).CompareTo obj
                | _ -> 1

        override self.Equals obj =
            (=) 0 <| (self :> IComparable).CompareTo obj
        override self.GetHashCode() = id.GetHashCode()
        override self.ToString() = $"{id}.{name}"

    type MovieName =
        | OneLang of string
        | BiLang of string * string

    type Movie(id : int, name : MovieName, actors : Actor Set, tags : Tag Set, director : Director Set, score : float option) =
        let id = id
        let mutable name = name
        let mutable actors = actors
        let mutable tags = tags
        let mutable director = director
        let mutable score = score

        new(id, name) = Movie(id, name, Set.empty, Set.empty, Set.empty, None)

        member self.Id = id
        member self.Name
            with get() = name
            and set value = name <- value
        member self.AddName name =
            match self.Name with
            | OneLang oldName ->
                self.Name <- BiLang (oldName, name)
            | _ -> ()
        member self.Actors
            with get() = actors
            and set value = actors <- value
        member self.Tags
            with get() = tags
            and set value = tags <- value
        member self.Director
            with get() = director
            and set value = director <- value
        member self.Score
            with get() = score
            and set value = score <- value

        member self.NonOptionScore =
            match self.Score with
            | Some s -> s
            | None -> -1.
        member self.GetNames =
            match self.Name with
            | OneLang name -> name, ""
            | BiLang (fst, snd) -> fst, snd

        member self.NamesToString =
            match self.GetNames with
            | name, "" -> name
            | fst, snd -> $"{fst} ({snd})"
        member self.ScoreToString =
            match self.Score with
            | Some score -> sprintf "%.4f" score
            | None -> "None"

        interface IComparable<Movie> with
            override self.CompareTo obj = compare self.Id obj.Id
        interface IComparable with
            override self.CompareTo obj =
                match obj with
                | null -> 1
                | :? Movie as obj -> (self :> IComparable<_>).CompareTo obj
                | _ -> 1

        override self.Equals obj =
            match obj with
            | :? Movie as obj -> self.Id = obj.Id
            | _ -> false
        override self.GetHashCode() = id.GetHashCode()
        override self.ToString() =
            let name = match self.Name with | OneLang name -> name | BiLang (fst, snd) -> $"{fst}/{snd}"
            let director = String.concat "\t" (Set.map (fun (a : Director) -> a.FullName) self.Director)
            let tags = String.concat "\t" (Set.map (fun (t : Tag) -> t.Name) self.Tags)
            let actors = String.concat "\t" (Set.map (fun (a : Actor) -> a.FullName) self.Actors)

            $"{id}. {name}: {score}
    Director: {director}
    Tags:
        -> {tags}
    Actors :
        -> {actors}"
