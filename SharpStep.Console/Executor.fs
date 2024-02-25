namespace SharpStep.Console

open SharpStep.Core
open System.Reactive.Linq


module Executor =
    let execute (command: Command) : StdOut =
        match command with
        | Handshake (Version version) ->
            version
            |> sprintf "st3p version %d ok"
            |> Observable.Return
        | Identify ->
            [ "name sharp-step"
              "author artfuldev<hello@artful.dev>"
              "version 0.1"
              "url https://github.com/artfuldev/sharp-step"
              "ok" ]
            |> Observable.ToObservable
            |> Observable.map (sprintf "identify %s")
        | Move _ -> command |> sprintf "%O" |> Observable.Return
        | Quit -> Observable.Empty()
