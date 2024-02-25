namespace SharpStep.Console

open SharpStep.Core
open System.Reactive.Linq
open System.Threading.Tasks
open System

module Executor =

    let rand = Random()

    let private indices (Board board) : (int * int) list =
        board
        |> List.mapi (fun i row ->
            row
            |> List.mapi (fun j cell ->
                match cell with
                | Playable -> Some(i, j)
                | _ -> None))
        |> List.collect id
        |> List.choose id

    let private move (board: Board) : Task<Response option> =
        task {
            let playable = indices board

            if List.isEmpty playable then
                return None
            else
                let index = rand.Next(playable.Length)
                let (i, j) = playable.[index]
                return (j, i) |> BestMove |> Some
        }

    let execute (command: Command) : StdOut =
        match command with
        | Handshake version ->
            version
            |> ReturnHandshake
            |> Response.strings
            |> Observable.ToObservable
        | Identify ->
            ("sharp-step", "0.1", "artfuldev<hello@artful.dev>", "https://github.com/artfuldev/sharp-step")
            |> Identification
            |> Response.strings
            |> Observable.ToObservable
        | Move ((board, _), _) ->
            Observable.StartAsync(fun () -> move board)
            |> Observable.choose id
            |> Observable.map Response.strings
            |> Observable.map Observable.ToObservable
            |> Observable.Concat
        | Quit -> Observable.Empty()
