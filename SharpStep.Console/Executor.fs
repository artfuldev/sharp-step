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
        | Move ((board, side), time) ->
            Observable.StartAsync(fun () -> Solver.random ((board, side), time))
            |> Observable.choose id
            |> Observable.map BestMove
            |> Observable.map Response.strings
            |> Observable.map Observable.ToObservable
            |> Observable.Concat
        | Quit -> Observable.Empty()
