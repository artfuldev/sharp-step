namespace SharpStep.Console

open System
open System.Reactive.Linq
open System.Reactive.Disposables
open System.Threading

type StdIn = IObservable<string>

type Inputs = private Inputs of StdIn

module Inputs =
    let create _ =
        Inputs(
            Observable.Create<string> (fun (observer: IObserver<string>) ->
                let running = ref true

                let readLines () =
                    while !running do
                        let line = Console.ReadLine()

                        if line = null then
                            running := false
                            observer.OnCompleted()
                        else
                            observer.OnNext(line)

                // Start the readLines function in a background thread.
                let thread = new Thread(ThreadStart(readLines))
                thread.Start()

                // Return a disposable that stops reading.
                Disposable.Create(fun () -> running := false))
        )
