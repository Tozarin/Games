namespace IMDBSolver

open System

module DataClasses =
    type Person(id : int, fullName : string, birthYear : int option, deathYear : int option) =
        let id = id
        let fullName = fullName
        let birthYear = birthYear
        let deathYear = deathYear

        member self.Id = id
        member self.FullName = fullName
        member self.BirthYear = birthYear
        member self.DeathYear = deathYear

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
        override self.ToString() =
            let yearToString = function | Some year -> $"{year}" | None -> "#"
            $"{id}. {fullName} ({yearToString birthYear} - {yearToString deathYear})"

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

    type Tag(id : int, name : string) =
        let id = id
        let name = name

        member self.Id = id
        member self.Name = name

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
        | RU of string
        | US of string
        | BiLang of string * string

    type Movie(id : int, name : MovieName, actors : Actor Set, tags : (Tag * float) Set, director : Director option, score : float option) =
        let id = id
        let name = name
        let mutable actors = actors
        let mutable tags = tags
        let mutable director = director
        let mutable score = score

        new(id, name) = Movie(id, name, Set.empty, Set.empty, None, None)

        member self.Id = id
        member self.Name = name
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

        member self.AllNames =
            match name with
            | RU name | US name -> [name]
            | BiLang (ru, us) -> [ru; us]

        override self.Equals obj =
            match obj with
            | :? Movie as obj -> self.Id = obj.Id
            | _ -> false
        override self.GetHashCode() = id.GetHashCode()
        override self.ToString() =
            let name = List.head self.AllNames
            let director = match self.Director with | Some d -> d.FullName | None -> "___"
            let tags = String.concat "\t" (Set.map (fun (t : Tag, _) -> t.Name) self.Tags)
            let actors = String.concat "\t" (Set.map (fun (a : Actor) -> a.FullName) self.Actors)

            $"{id}. {name}: {score}
    Director: {director}
    Tags:
        -> {tags}
    Actors :
        -> {actors}"
