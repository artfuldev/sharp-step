namespace SharpStep.Console

open System.Reactive.Linq

module App =
    let run (Inputs stdin) : Outputs =
        let stdout = stdin
        let stderr = Observable.Never<string>()
        let exit = Observable.Never<int>()
        Outputs.create stdout stderr exit
