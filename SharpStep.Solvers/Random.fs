﻿namespace SharpStep.Solvers

open System
open SharpStep.Core
open SharpStep.Core.Board
open System.Reactive.Linq
open Helpers

module Random =

    let private shuffle list =
        let rand = Random()
        let array = Seq.toArray list
        let n = Array.length array

        for i in [ n - 1 .. -1 .. 1 ] do
            let j = rand.Next(i + 1)
            let tmp = array.[i]
            array.[i] <- array.[j]
            array.[j] <- tmp

        Array.toSeq array

    let position board =
        task {
            return
                board
                |> positions
                |> Seq.filter ((flip at) board >> (=) Playable)
                |> shuffle
                |> Seq.tryHead
        }

    let solver ((board, _, _, _): Specification) =
        Observable.StartAsync(fun () -> position board)
        |> Observable.choose id
