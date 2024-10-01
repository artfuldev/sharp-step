namespace SharpStep.Console

open FParsec
open SharpStep.Core

module Parser =
    let private side: Parser<Side, unit> = choice [ pchar 'x' >>% X; pchar 'o' >>% O ]

    let private cell: Parser<Cell, unit> =
        choice [ pchar '_' >>% Playable; pchar '.' >>% Unplayable; side |>> Played ]

    let private compressed: Parser<Cell list, unit> =
        ((opt pint32) .>>. cell)
        |>> function
            | (Some count, cell) -> List.replicate count cell
            | (None, cell) -> [ cell ]

    let private row: Parser<Cell list, unit> = many1 compressed |>> List.collect id

    let private board: Parser<Board, unit> = sepEndBy1 row (pstring "/") |>> Board

    let private duration: Parser<Duration, unit> =
        pstring "ms:" >>. pint32 |>> Milliseconds

    let private time: Parser<Time, unit> =
        choice
            [ pstring "time " >>. duration |>> PerMove
              pstring "time-remaining " >>. duration |>> Remaining ]

    let private win: Parser<WinLength, unit> =
        pstring "win-length " >>. pint32 |>> Provided

    let private whitespace: Parser<string, unit> = many1Chars (anyOf " \t\r\n")

    type private MoveOption =
        | TimeOption of Time
        | WinLengthOption of WinLength

    module private MoveOption =

        let toTime =
            function
            | TimeOption x -> Some x
            | _ -> None

        let toWinLength =
            function
            | WinLengthOption x -> Some x
            | _ -> None

    let private moveOption: Parser<MoveOption, unit> =
        opt (whitespace) >>. choice [ time |>> TimeOption; win |>> WinLengthOption ]

    let private handshake: Parser<Command, unit> =
        pstring "st3p version " >>. pint32 |>> Version |>> Handshake

    let private identify: Parser<Command, unit> = pstring "identify" >>% Identify

    let private move: Parser<Command, unit> =
        (pstring "move") >>. opt (whitespace) >>. board
        .>>. (opt (whitespace) >>. side)
        .>>. (many moveOption)
        |>> (fun ((board, side), moveOptions) ->
            let time =
                moveOptions
                |> List.choose MoveOption.toTime
                |> List.tryLast
                |> Option.defaultValue Infinite

            let winLength =
                moveOptions
                |> List.choose MoveOption.toWinLength
                |> List.tryLast
                |> Option.defaultValue Default in

            Move(board, side, time, winLength))

    let private quit: Parser<Command, unit> = pstring "quit" >>% Quit

    let private command: Parser<Command, unit> =
        choice [ handshake; identify; move; quit ]

    let parse str =
        match run command str with
        | Success(cmd, _, _) -> Result.Ok cmd
        | Failure(_, _, _) -> Result.Error str

    let debug (result: Result<Command, string>) =
        match result with
        | Result.Ok cmd -> sprintf "Known command: %A" cmd
        | Result.Error msg -> sprintf "Unknown command: %A" msg
