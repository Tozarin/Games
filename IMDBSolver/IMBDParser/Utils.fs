namespace Parser

module Operators =
     let (><>) opt f = Option.bind f opt
