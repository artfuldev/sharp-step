namespace SharpStep.Solvers

open System
open SharpStep.Core

type Specification = (Board * Side) * Time

type Solver = Func<Specification, IObservable<Position>>
