namespace SharpStep.Core

type Name = string

type Author = string

type EngineVersion = string

type Url = string

type RowIndex = int

type ColumnIndex = int

type Move = ColumnIndex * RowIndex

type Response =
    | ReturnHandshake of Version
    | Identification of Name * EngineVersion * Author * Url
    | BestMove of Move

module Response =

    let private column (index: int) =
        let rec build index result =
            if index < 0 then
                result
            else
                let remainder = index % 26
                let char = char (97 + remainder)
                let index' = index / 26 - 1
                build index' (string char + result)

        build index ""

    let strings =
        function
        | ReturnHandshake (Version (version)) -> [ sprintf "st3p version %d ok" version ]
        | Identification (name, version, author, url) ->
            [ name; version; author; url ]
            |> List.zip [ "name"
                          "version"
                          "author"
                          "url" ]
            |> List.map (fun (key, value) -> sprintf "%s %s" key value)
            |> List.rev
            |> List.append (List.singleton "ok")
            |> List.rev
            |> List.map (sprintf "identify %s")
        | BestMove (col, row) -> [ sprintf "best %s%d" (column col) (row + 1) ]
