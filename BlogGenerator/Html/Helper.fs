namespace Html

module Helper =

    // HTML node output format
    type FmtType =
        | MulLine of int // indent level
        | SinLine of int // indent level
        | Inline
    
    let getIndent indent =
        match indent with
            | MulLine(cnt) -> cnt
            | SinLine(cnt) -> cnt
            | Inline -> 0

    // calculate the spaces for code aligment
    let genSpaces count =
        List.init count id
            |> List.map (fun i -> "    ")
            |> String.concat ""

