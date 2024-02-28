namespace SharpStep.Core

type Board = Board of Cell list list

module Board =

    let create size =
        let row = List.init size (fun _ -> Playable)
        let cells = List.init size (fun _ -> row)
        Board cells

    let positions (Board cells) =
        cells
        |> Seq.mapi (fun i row -> row |> Seq.mapi (fun j _ -> Position(j, i)))
        |> Seq.collect id

    let at (Position (j, i)) (Board cells) = cells.[i].[j]

    let play side position (Board cells) =
        let cells' = cells |> List.mapi (fun i row -> row |> List.mapi (fun j cell -> if Position(j, i) = position then Played side else cell))
        Board cells'
