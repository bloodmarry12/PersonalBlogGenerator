/// Markdown : the Markdown document parser
namespace Markdown

/// Library user interface
module Interface =

    open Syntax.Definitions
    open Block
    open System.IO

    let loadMarkdownFile path =
        let stream = File.OpenText(path)

        seq {
            while (not stream.EndOfStream) do
                yield stream.ReadLine()
        } |> List.ofSeq

    let parseMarkdownFile path =
        loadMarkdownFile path
            |> parseBlocks
            |> List.ofSeq

