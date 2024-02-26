namespace SharpStep.Core

open System
open System.Threading.Tasks

type Problem = (Board * Side) * Time

type Solver = Func<Problem, Task<Position option>>

module Solver =

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

    let random (((board, _), _): Problem) =
        task {
            return
                board
                |> Position.positions
                |> shuffle
                |> Seq.tryHead
        }
