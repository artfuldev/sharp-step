namespace SharpStep.Solvers

open System
open SharpStep.Core

type Specification = Board * Side * Time * WinLength

type Solver = Func<Specification, IObservable<Position>>
