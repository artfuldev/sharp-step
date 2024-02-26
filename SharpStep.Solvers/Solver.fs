namespace SharpStep.Solvers

open System
open System.Threading.Tasks
open SharpStep.Core

type Specification = (Board * Side) * Time

type Solver = Func<Specification, Task<Position option>>
