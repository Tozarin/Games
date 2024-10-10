namespace IMDBSolver

module Operators =
     let (><>) opt f = Option.bind f opt
