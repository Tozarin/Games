namespace IMDBSolver

open System

module Loggers =

    type ILogger =
        abstract member Log : string -> unit

    type EmptyLogger =

        new() = {}

        interface ILogger with
            override self.Log _ = ()

    type SimpleLogger =

        val placeFrom : string
        val mutable lastTimeLog : DateTime

        new(placeFrom) = {
            placeFrom = placeFrom
            lastTimeLog = DateTime.Now
        }

        interface ILogger with
            override self.Log mes =
                let newTime = DateTime.Now

                printfn $"{self.placeFrom} since last log {newTime - self.lastTimeLog}: {mes}"
                self.lastTimeLog <- newTime
