namespace Parser

open System
open System.Collections
open System.Collections.Generic
open System.IO
open Loggers
open Microsoft.FSharp.Core

module FileSignature =
    type extn =
        | TXT
        | TSV
        | CSV

        override self.ToString() =
            match self with
            | TXT -> "txt"
            | TSV -> "tsv"
            | CSV -> "csv"

    type file =
        {
            dirPath : string
            fileName : string
            extn : extn
        }

        override self.ToString() =
            $"{self.dirPath.Trim('\\')}\\{self.fileName}.{self.extn}"

module Reader =
    open FileSignature

    type Reader(filePath : string) =
        let filePath = filePath
        let lines = seq {
            use sr = new StreamReader(filePath)
            while not sr.EndOfStream do
                yield sr.ReadLine()
        }

        new(file : file) = Reader(file.ToString())
        new(dirPath, fileName) = Reader($"{dirPath}\\{fileName}")
        new(dirPath, fileName, extn) = Reader($"{dirPath}\\{fileName}.{extn}")

        member self.Lines = lines

    [<AbstractClass>]
    type Parser<'splitted, 'ret> =
        inherit Reader

        val logger : ILogger

        new(filePath : string, logger) = { inherit Reader(filePath); logger = logger }
        new(file : file, logger) = { inherit Reader(file); logger = logger }
        new(dirPath, fileName, logger) = { inherit Reader(dirPath, fileName); logger = logger }
        new(dirPath, fileName, extn, logger) = { inherit Reader(dirPath, fileName, extn); logger = logger }

        abstract returnValue : unit -> 'ret
        abstract split : string -> 'splitted
        abstract preFunction : IEnumerator<string> -> unit
        abstract iterFunction : 'splitted -> unit

        member self.ReadAndIter() =
            self.logger.Log("START")

            let lines = self.Lines.GetEnumerator()
            ignore <| lines.MoveNext()

            self.preFunction(lines)

            while lines.MoveNext() do
                lines.Current |> self.split |> self.iterFunction

                (*let task = async { line |> self.split |> self.iterFunction }
                Async.Start task*)

            self.logger.Log("END")
            self.returnValue()
