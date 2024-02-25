namespace SharpStep.Console


open System.Reactive.Linq
open Parser

module App =

    let run (Inputs stdin) : Outputs =
        let parsed =
            stdin
            |> Observable.map parse
            |> Observable.Publish
            |> Observable.RefCount

        let stdout =
            parsed
            |> Observable.filter Result.isOk
            |> Observable.map debug

        let stderr =
            parsed
            |> Observable.filter Result.isError
            |> Observable.map debug

        let exit = Observable.Never<int>()
        Outputs.create stdout stderr exit
