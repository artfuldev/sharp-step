namespace SharpStep.Solvers

open SharpStep.Core
open Helpers

module Evaluation =
    let internal isTerminal length board =
        let drawn =
            (Board.positions board
             |> Seq.filter (((flip Board.at) board) >> ((=) Playable))
             |> Seq.isEmpty)

        let won =
            Win.positions board length
            |> Seq.map Seq.toList
            |> Seq.exists ((flip Win.winner) board >> ((<>) None))

        drawn || won

    let internal multiplier =
        function
        | X -> 1
        | O -> -1

    let internal difference board positions =
        let x =
            positions
            |> Seq.filter ((flip Board.at) board >> ((=) (Played X)))
            |> Seq.length

        let o =
            positions
            |> Seq.filter ((flip Board.at) board >> ((=) (Played O)))
            |> Seq.length

        (x - o)

    let internal heuristic board =
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
