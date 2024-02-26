namespace SharpStep.Console

open SharpStep.Core
open System.Reactive.Linq
open SharpStep.Solvers

module Executor =

    let execute (command: Command) : StdOut =
        match command with
        | Handshake version ->
            version
            |> ReturnHandshake
            |> Response.strings
            |> Observable.ToObservable
        | Identify ->
            "0.9"
            |> Response.identify
            |> Response.strings
            |> Observable.ToObservable
        | Move ((board, side), time) ->
            Observable.StartAsync(fun () -> Random.solver ((board, side), time))
            |> Observable.choose id
            |> Observable.map BestMove
            |> Observable.map Response.strings
            |> Observable.map Observable.ToObservable
            |> Observable.Concat
        | Quit -> Observable.Empty()
