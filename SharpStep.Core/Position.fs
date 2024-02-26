namespace SharpStep.Core

type RowIndex = int

type ColumnIndex = int

type Position = Position of ColumnIndex * RowIndex

module Position =
    let private column (index: int) =
        let rec build index result =
            if index < 0 then
                result
            else
                let remainder = index % 26
                let char = char (97 + remainder)
                let index' = index / 26 - 1
                build index' (string char + result)

        build index ""

    let string (Position (col, row)) = sprintf "%s%d" (column col) (row + 1)

    let positions (Board cells) =
        cells
        |> Seq.mapi (fun i row -> row |> Seq.mapi (fun j _ -> Position(j, i)))
        |> Seq.collect id

    let at (Board cells) (Position (j, i)) = cells.[i].[j]
