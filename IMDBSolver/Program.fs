namespace IMDBSolver

open CI
open Database

module Main =
    [<EntryPoint>]
    let main args =
        assert( not <| Array.isEmpty args)
        let pathToDatabase = args[0]

        //imdb2
        let connectionString = $"Host=localhost;Port=5432;Database={pathToDatabase};Username=postgres;Password=admin;Timeout=600;CommandTimeout=600;Include Error Detail=true;"
        let factory = ContexFactory(connectionString)
        use db = factory.NewContex()

        SimpleCI2(factory).startLoop()

        0
