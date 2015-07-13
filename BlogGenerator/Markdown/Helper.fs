namespace Markdown

module Helper =

    let toString chars =
        System.String(chars |> Array.ofList)

    let toCharList (str : string) =
        str |> List.ofSeq

