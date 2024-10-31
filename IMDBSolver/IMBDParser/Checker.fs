namespace Parser

open System.IO

module Checker =
    let dirExistCheck = Directory.Exists

    let fileNames =
        [
            PersonsReader.personsCodesFileName
            PersonsReader.personsNamesFileName
            TagsReadier.tagScoresFileName
            MoviesReader.fileName
            RatingsReader.ratingsFileName
            TagsReadier.idsMapperFileName
            TagsReadier.tagCodesFileName
        ]

    let filesExistCheck path =
        let getFilename (filename : string) =
            let withExtn = Array.last <| filename.Split [|'\\'|]
            Array.head <| withExtn.Split [|'.'|]
        let files = Directory.GetFiles path
                    |> Set.ofArray
                    |> Set.map getFilename
        List.fold (fun acc filename -> acc && files.Contains filename) true fileNames
