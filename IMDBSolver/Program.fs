namespace IMDBSolver

open CI
open Database.Contex

module Main =
    [<EntryPoint>]
    let main args =
        assert( not <| Array.isEmpty args)
        let pathToDatabase = args[0]

        //imdb2
        let connectionString = $"Host=localhost;Port=5432;Database={pathToDatabase};Username=postgres;Password=admin;"
        let factory = DbFactory(connectionString)
        use db = factory.NewConnection()
        ignore <| db.Database.EnsureCreated()

        SimpleCI2(factory).startLoop()

        0
