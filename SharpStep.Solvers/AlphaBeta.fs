namespace SharpStep.Solvers

open SharpStep.Core
open Helpers
open Evaluation
open System
open System.Reactive.Linq

module AlphaBeta =
    let rec private evaluate alpha beta depth side board =
        if depth = 0 || isTerminal None board then
            heuristic (board) * (float (multiplier side))
        else
            let rec loop positions alpha value =
                match positions with
                | [] -> value
                | pos :: rest ->
                    let board' = Board.play side pos board
                    let side' = Side.other side
                    let depth' = depth - 1

                    let value' =
                        max value (-evaluate -beta -alpha depth' side' board')

                    if value' >= beta then
                        value'
                    else
                        loop rest (max alpha value') value'

            let positions =
                Board.positions (board)
                |> Seq.filter ((flip Board.at) board >> (=) Playable)
                |> Seq.toList

            loop positions alpha -infinity

    let solver (((board, side), _): Specification) : IObservable<Position> =
        let moves =
            Board.positions board
            |> Seq.filter ((flip Board.at) board >> (=) Playable)

        let depth = moves |> Seq.length

        if depth = 0 then
            Observable.Empty()
        else
            moves
            |> Seq.map (fun pos ->
                let board' = Board.play side pos board
                let side' = Side.other side
                (pos, -(evaluate infinity -infinity depth side' board')))
            |> Seq.maxBy snd
            |> fst
            |> Observable.Return
