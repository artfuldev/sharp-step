namespace SharpStep.Solvers

open System
open SharpStep.Core
open SharpStep.Core.Position
open System.Reactive.Linq

module Random =

    let private shuffle (list: seq<'a>) : seq<'a> =
        let rand = Random()
        let array = Seq.toArray list
        let n = Array.length array

        for i in [ n - 1 .. -1 .. 1 ] do
            let j = rand.Next(i + 1)
            let tmp = array.[i]
            array.[i] <- array.[j]
            array.[j] <- tmp

        Array.toList array

    let solver (((board, _), _): Specification) =
        board
        |> positions
        |> Seq.filter ((at board) >> ((=) Playable))
        |> shuffle
        |> Seq.tryHead
        |> Observable.Return
        |> Observable.choose id
