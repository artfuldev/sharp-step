namespace SharpStep.Core

type Version = Version of int

type Duration = Milliseconds of int

type Time =
    | PerMove of Duration
    | Remaining of Duration
    | Infinite

type WinLength =
    | Provided of int
    | Default

module WinLength =
    let length = function | Default -> None | Provided x -> Some x

type Command =
    | Handshake of Version
    | Identify
    | Move of (Board * Side * Time * WinLength)
    | Quit
