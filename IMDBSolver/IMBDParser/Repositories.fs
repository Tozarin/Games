namespace Parser


open DataClasses
open MyConcurrentDictionary

module Repositories =
    type Repository<'t when 't : equality>() =
        let data = new PresetConcurrentDictionary<int, 't>()

        member self.get (obj : 't) =
            let id = obj.GetHashCode()
            self.getById id
        member self.getById id =
            if data.ContainsKey id then Some data[id]
            else None
        member self.put (obj : 't) =
            let id = obj.GetHashCode()
            if not <| data.ContainsKey id then data[id] <- obj
        member self.getOrPut obj =
            let id = obj.GetHashCode()
            if data.ContainsKey id then data[id]
            else
                data[id] <- obj
                obj
        member self.change (obj : 't) (map : 't -> unit) =
            let id = obj.GetHashCode()
            map data[id]

        member self.Data = seq { for kv in data -> kv.Value }

    type TagsRepository = Repository<Tag>
    type MoviesRepository = Repository<Movie>
    type PersonsRepository = Repository<Person>
    type ActorsRepository = Repository<Actor>
    type DirectorsRepository = Repository<Director>
