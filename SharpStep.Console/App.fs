namespace SharpStep.Console

open SharpStep.Core
open System.Reactive.Linq
open Parser
open Executor

module App =

    let run (Inputs stdin) : Outputs =
        let _warmup =
            "move 3_/3_/3_ x"
            |> parse
            |> Result.map execute
            |> Result.map (Observable.subscribe (fun _ -> ()))
            |> ignore

        let parsed =
            stdin
            |> Observable.map parse
            |> Observable.Publish
            |> Observable.RefCount

        let commands =
            parsed
            |> Observable.filter Result.isOk
            |> Observable.map Result.toOption
            |> Observable.choose id
            |> Observable.Publish
            |> Observable.RefCount

        let stdout =
            commands
            |> Observable.filter ((<>) Quit)
            |> Observable.map execute
            |> Observable.Switch

        let stderr =
            parsed
            |> Observable.filter Result.isError
            |> Observable.map debug

        let exit =
            commands
            |> Observable.filter ((=) Quit)
            |> Observable.map (fun _ -> 0)

        Outputs.create stdout stderr exit
