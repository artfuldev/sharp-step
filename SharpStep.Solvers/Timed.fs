namespace SharpStep.Solvers

open System
open System.Reactive.Linq
open SharpStep.Core
open SharpStep.Core.Position

module Timed =
    let rec private factorial x =
        if x <= 0 then
            1
        else
            x * factorial (x - 1)

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
                    |> float
                    |> (*) 0.5
                    |> Math.Ceiling
                    |> int

                Observable.Timer(TimeSpan.FromMilliseconds((float ms) / (float (factorial (playable - 1)))))

        ((board, side), time)
        |> solver.Invoke
        |> Observable.scan (fun _ -> Some) None
        |> fun x -> Observable.TakeUntil(x, timeout)
        |> Observable.LastAsync
        |> Observable.choose id
