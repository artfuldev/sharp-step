namespace SharpStep.Console

open SharpStep.Core
open System.Reactive.Linq
open Parser

module App =

    let run (Inputs stdin) : Outputs =
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
            |> Observable.map string

        let stderr =
            parsed
            |> Observable.filter Result.isError
            |> Observable.map debug

        let exit =
            commands
            |> Observable.filter ((=) Quit)
            |> Observable.map (fun _ -> 0)

        Outputs.create stdout stderr exit
