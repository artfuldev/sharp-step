namespace SharpStep.Core

type Side =
    | X
    | O

module Side =
    let other =
        function
        | X -> O
        | O -> X
