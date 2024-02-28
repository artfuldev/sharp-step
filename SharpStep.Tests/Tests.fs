module Tests

open System
open SharpStep.Core
open SharpStep.Solvers
open SharpStep.Solvers.Helpers
open Xunit


let board = Board.create 3

let expected =
    ((Some 3)
     |> Win.positions board
     |> Seq.map List.ofSeq
     |> List.ofSeq
     |> List.map (List.map Position.string)
     |> Set.ofList
     |> Set.map set)

[<Fact>]
let ``Winning positions contains all rows`` () =
    let rows =
        [ [ Position(0, 0)
            Position(1, 0)
            Position(2, 0) ]
          [ Position(0, 1)
            Position(1, 1)
            Position(2, 1) ]
          [ Position(0, 2)
            Position(1, 2)
            Position(2, 2) ] ]
        |> List.map (List.map Position.string)
        |> List.map set

    Assert.True(rows |> List.forall ((flip Set.contains) expected))

[<Fact>]
let ``Winning positions contains all columns`` () =
    let columns =
        [ [ Position(0, 0)
            Position(0, 1)
            Position(0, 2) ]
          [ Position(1, 0)
            Position(1, 1)
            Position(1, 2) ]
          [ Position(2, 0)
            Position(2, 1)
            Position(2, 2) ] ]
        |> List.map (List.map Position.string)
        |> List.map set

    Assert.True(
        columns
        |> List.forall ((flip Set.contains) expected)
    )

[<Fact>]
let ``Winning positions contains all diagonals`` () =
    let diagonals =
        [ [ Position(0, 0)
            Position(1, 1)
            Position(2, 2) ]
          [ Position(2, 0)
            Position(1, 1)
            Position(0, 2) ] ]
        |> List.map (List.map Position.string)
        |> List.map set

    Assert.True(
        diagonals
        |> List.forall ((flip Set.contains) expected)
    )

let cells =
    [ [ (Played X)
        Unplayable
        (Played X)
        (Played O)
        (Playable) ] ]

[<Fact>]
let ``If any cell is unplayable, there's no winner`` =
    let winner =
        Win.winner
            [ Position(0, 0)
              Position(2, 0)
              Position(1, 0) ]
            (Board cells)

    Assert.StrictEqual(winner, None)

[<Fact>]
let ``If all cells are played by a single player, that player wins`` () =
    let winner =
        Win.winner [ Position(0, 0); Position(2, 0) ] (Board cells)

    Assert.StrictEqual(winner, Some X)

[<Fact>]
let ``If all cells are not played by a single player, no one wins`` () =
    let winner =
        Win.winner
            [ Position(0, 0)
              Position(2, 0)
              Position(3, 0) ]
            (Board cells)

    Assert.StrictEqual(winner, None)

[<Fact>]
let ``If any cell is playable, there's no winner`` =
    let winner =
        Win.winner
            [ Position(0, 0)
              Position(2, 0)
              Position(4, 0) ]
            (Board cells)

    Assert.StrictEqual(winner, None)
