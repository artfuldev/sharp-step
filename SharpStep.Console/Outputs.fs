namespace SharpStep.Console

open System
open System.Reactive.Disposables

type StdOut = IObservable<string>

type StdErr = IObservable<string>

type Exit = IObservable<int>

type Outputs = private Outputs of StdOut * StdErr * Exit

module Outputs =
    let create out err exit = Outputs(out, err, exit)

    let handle (Outputs (out, err, exit)) =
        out.Subscribe(fun (x) -> Console.WriteLine(x))
        |> ignore

        err.Subscribe(fun (x: string) -> Console.Error.WriteLine(x))
        |> ignore

        exit.Subscribe(fun x -> Environment.Exit(x))
        |> ignore
