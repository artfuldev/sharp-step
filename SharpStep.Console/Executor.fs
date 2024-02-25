namespace SharpStep.Console

open SharpStep.Core
open System.Reactive.Linq

module Executor =
    let execute (command: Command) : StdOut =
        match command with
        | Handshake version ->
            version
            |> ReturnHandshake
            |> Response.strings
            |> Observable.ToObservable
        | Identify ->
            ("sharp-step", "0.1", "artfuldev<hello@artful.dev>", "https://github.com/artfuldev/sharp-step")
            |> Identification
            |> Response.strings
            |> Observable.ToObservable
        | Move _ -> Observable.Empty()
        | Quit -> Observable.Empty()
