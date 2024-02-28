namespace SharpStep.Solvers

open SharpStep.Core
open SharpStep.Core.Board

module Win =

    let positions (Board cells) (length: int option) : Position seq seq =
        let length =
            defaultArg length cells.Length |> min cells.Length

        let size = cells.Length

        let diagonal, antiDiagonal =
            [ 0 .. size - 1 ]
            |> List.map (fun i -> (i, i), (i, size - i - 1))
            |> List.unzip

        let positions =
            [ for i in 0 .. size - 1 do
                  for j in 0 .. size - length do
                      let row =
                          [ for k in 0 .. length - 1 -> (i, j + k) ]

                      let column =
                          [ for k in 0 .. length - 1 -> (j + k, i) ]

                      yield row
                      yield column ]
            @ [ for i in 0 .. size - length do
                    yield diagonal.[i..i + length - 1]
                    yield antiDiagonal.[i..i + length - 1] ]

        positions
        |> Seq.map (Seq.map (fun (x, y) -> (Position(y, x))))

    let winner (positions: Position list) (board: Board) : Side option =
        let rec check positions side =
            match side with
            | None ->
                match positions with
                | [] -> None
                | head :: tail ->
                    match at head board with
                    | Played player -> check tail (Some player)
                    | _ -> None
            | Some candidate ->
                match positions with
                | [] -> Some candidate
                | head :: tail ->
                    match at head board with
                    | Played player when player = candidate -> check tail (Some player)
                    | _ -> None

        check positions None
