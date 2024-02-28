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
            "1.1.2"
            |> Response.identify
            |> Response.strings
            |> Observable.ToObservable
        | Move ((board, side), time) ->
            ((board, side), time)
            |> Timed.solver AlphaBeta.solver
            |> Observable.map BestMove
            |> Observable.map Response.strings
            |> Observable.map Observable.ToObservable
            |> Observable.Switch
        | Quit -> Observable.Empty()
