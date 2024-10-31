namespace Parser

open System
open System.Collections.Concurrent
open System.Collections.Generic

module MyConcurrentDictionary =

    let initSize = 1200
    let countOfThreads = Environment.ProcessorCount * 2

    type PresetConcurrentDictionary<'key, 'value> =
        inherit ConcurrentDictionary<'key, 'value>

        new() = { inherit ConcurrentDictionary<'key, 'value>(countOfThreads, initSize)  }

        member self.toNormal() =
            new Dictionary<'key, 'value>(self, self.Comparer)
