namespace SharpStep.Solvers

open SharpStep.Core
open System.Reactive.Linq
open Helpers
open Evaluation
open System

module Negamax =

    let rec evaluate length (board: Board) (side: Side) (depth: int) : float =
        if depth = 0 || isTerminal length board then
            heuristic length (board) * (float (multiplier side))
        else
            Board.positions (board)
            |> Seq.filter ((flip Board.at) board >> (=) Playable)
            |> Seq.toList
            |> List.map (fun pos ->
                let board' = Board.play side pos board
                let side' = Side.other side
                let depth' = depth - 1
                -(evaluate length board' side' depth'))
            |> List.max

    let solver ((board, side, _, winLength): Specification) : IObservable<Position> =
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
                (pos, -(evaluate (WinLength.length winLength) board' side' depth)))
            |> Seq.maxBy snd
            |> fst
            |> Observable.Return
