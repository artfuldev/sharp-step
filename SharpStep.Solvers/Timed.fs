namespace SharpStep.Solvers

open System
open System.Reactive.Linq
open SharpStep.Core
open SharpStep.Core.Board
open Helpers

module Timed =

    let solver (solver: Solver) ((board, side, time, winLength): Specification) : IObservable<Position> =

        let timeout =
            match time with
            | Infinite -> Observable.Never()
            | PerMove (Milliseconds ms) -> Observable.Timer(TimeSpan.FromMilliseconds(float ms))
            | Remaining (Milliseconds ms) ->
                let playable =
                    board
                    |> positions
                    |> Seq.filter ((flip at) board >> (=) Playable)
                    |> Seq.length

                Observable.Timer(TimeSpan.FromMilliseconds((float ms) / (float playable)))

        (board, side, time, winLength)
        |> solver.Invoke
        |> Observable.scan (fun _ -> Some) None
        |> fun x -> Observable.TakeUntil(x, timeout)
        |> Observable.LastAsync
        |> Observable.choose id
