namespace SharpStep.Core

type Cell =
    | Unplayable
    | Playable
    | Played of Side
