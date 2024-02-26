namespace SharpStep.Core

type Name = string

type Author = string

type EngineVersion = string

type Url = string

type Response =
    | ReturnHandshake of Version
    | Identification of Name * EngineVersion * Author * Url
    | BestMove of Position

module Response =
    let identify version =
        Identification("sharp-step", version, "artfuldev<hello@artful.dev>", "https://github.com/artfuldev/sharp-step")

    let strings =
        function
        | ReturnHandshake (Version (version)) ->
            version
            |> sprintf "st3p version %d ok"
            |> List.singleton
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
        | BestMove position ->
            position
            |> Position.string
            |> sprintf "best %s"
            |> List.singleton
