namespace SharpStep.Console

open System
open System.Reactive.Disposables
open System.Threading

type StdOut = IObservable<string>

type StdErr = IObservable<string>

type Exit = IObservable<int>

type Outputs = private Outputs of StdOut * StdErr * Exit

module Outputs =
    let create stdout stderr exit = Outputs(stdout, stderr, exit)

    let private out = fun (x: string) -> Console.WriteLine(x)

    let private err =
        fun (x: string) -> Console.Error.WriteLine(x)

    let handle (Outputs (stdout, stderr, exit)) =
        stdout.Subscribe(out) |> ignore
        stderr.Subscribe(err) |> ignore
        exit.Subscribe(Environment.Exit) |> ignore
