namespace SharpStep.Solvers

open SharpStep.Core
open System.Reactive.Linq
open Helpers
open System

module Negamax =

    let private isTerminal length board =
        let drawn =
            (Board.positions board
             |> Seq.filter (((flip Board.at) board) >> ((=) Playable))
             |> Seq.isEmpty)

        let won =
            Win.positions board length
            |> Seq.map Seq.toList
            |> Seq.exists ((flip Win.winner) board >> ((<>) None))

        drawn || won

    let private multiplier =
        function
        | X -> 1
        | O -> -1

    let private difference board positions =
        let x =
            positions
            |> Seq.filter ((flip Board.at) board >> ((=) (Played X)))
            |> Seq.length

        let o =
            positions
            |> Seq.filter ((flip Board.at) board >> ((=) (Played O)))
            |> Seq.length

        (x - o)

    let private heuristic board =
        let positions = Win.positions board None

        let winner =
            positions
            |> Seq.map Seq.toList
            |> Seq.map ((flip Win.winner) board)
            |> Seq.tryFind ((<>) None)
            |> Option.flatten

        let size =
            match board with
            | Board cells -> cells.Length * cells.Length * cells.Length

        match winner with
        | Some X -> (float size)
        | Some O -> (float -size)
        | None ->
            let drawn =
                (Board.positions board
                 |> Seq.filter (((flip Board.at) board) >> ((=) Playable))
                 |> Seq.isEmpty)

            if drawn then
                0.0
            else
                positions |> Seq.sumBy (difference board) |> float

    let rec evaluate (board: Board) (side: Side) (depth: int) : float =
        if depth = 0 || isTerminal None board then
            heuristic (board) * (float (multiplier side))
        else
            Board.positions (board)
            |> Seq.filter ((flip Board.at) board >> (=) Playable)
            |> Seq.toList
            |> List.map (fun pos ->
                let board' = Board.play side pos board
                let side' = Side.other side
                let depth' = depth - 1
                -(evaluate board' side' depth'))
            |> List.max

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
                (pos, -(evaluate board' side' depth)))
            |> Seq.maxBy snd
            |> fst
            |> Observable.Return
