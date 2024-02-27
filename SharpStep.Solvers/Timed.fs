namespace SharpStep.Solvers

open System
open System.Reactive.Linq
open SharpStep.Core
open SharpStep.Core.Position

module Timed =
    let solver (solver: Solver) (((board, side), time): Specification) : IObservable<Position> =

        let timeout =
            match time with
            | Infinite -> Observable.Never()
            | PerMove (Milliseconds ms) -> Observable.Timer(TimeSpan.FromMilliseconds(float ms))
            | Remaining (Milliseconds ms) ->
                let playable =
                    board
                    |> positions
                    |> Seq.filter ((at board) >> ((=) Playable))
                    |> Seq.length

                Observable.Timer(TimeSpan.FromMilliseconds((float ms) / (float playable)))

        ((board, side), time)
        |> solver.Invoke
        |> Observable.scan (fun _ -> Some) None
        |> fun x -> Observable.TakeUntil(x, timeout)
        |> Observable.LastAsync
        |> Observable.choose id
