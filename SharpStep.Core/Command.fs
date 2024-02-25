namespace SharpStep.Core

type Version = Version of int

type Duration = Milliseconds of int

type Time =
    | PerMove of Duration
    | Remaining of Duration
    | Infinite

type Command =
    | Handshake of Version
    | Identify
    | Move of (Board * Side) * Time
    | Quit
