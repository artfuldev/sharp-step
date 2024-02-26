namespace SharpStep.Solvers

open System
open SharpStep.Core

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
        task {
            return
                board
                |> Position.positions
                |> shuffle
                |> Seq.filter ((Position.at board) >> ((=) Playable))
                |> Seq.tryHead
        }
