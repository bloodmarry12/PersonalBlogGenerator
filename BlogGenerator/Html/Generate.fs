namespace Html

module Generate =
    
    open Syntax.Definitions
    open Helper
    open Node
    open System.IO

    // generate HTML code from DocSpan
    let rec genSpan (out : TextWriter) indent = function
        | Literal(content) -> out.Write(content)
        | Strong(spans) ->
            genPairTagElem out Inline "strong" [] (fun i ->
                spans |> List.iter (genSpan out indent))
        | Emphasis(spans) ->
            genPairTagElem out Inline "em" [] (fun i ->
                spans |> List.iter (genSpan out indent))
        | HyperLink(spans, url) ->
            genPairTagElem out Inline "a" [" href", url] (fun i ->
                spans |> List.iter (genSpan out indent))
        | ImageLink(alt, url) ->
            genPairTagElem out Inline "img" [" src", url; "alt", alt] (fun i ->
                ())
        | InlineCode(code) ->
            out.Write("<code>" + code + "</code>")
        | HardBreak ->
            out.Write("<br>")

    // generate HTML code from DocBlock
    let rec genBlock (out : TextWriter) indent = function
        | Heading(size, spans) ->
            let Indent = SinLine(getIndent indent)
            genPairTagElem out Indent ("h" + size.ToString()) [] (fun i ->
                spans |> List.iter (genSpan out Inline))
        | Paragraph(spans) ->
            let Indent = MulLine(getIndent indent)
            genPairTagElem out Indent "p" [] (fun i ->
                out.Write(genSpaces(getIndent(indent) + 1))
                spans |> List.iter (genSpan out Indent)
                out.WriteLine())
        | CodeBlock(lines) ->
            let Indent = MulLine(getIndent indent)
            genPairTagElem out Indent "pre" [] (fun i ->
                lines |> List.iter (fun it -> out.WriteLine("    " + it)))
        | QuotedBlock(blocks) ->
            let Indent = MulLine(getIndent indent)
            genPairTagElem out Indent "blockquote" [] (fun i ->
                let ContentIndent = MulLine(i + 1)
                blocks |> List.iter (genBlock out ContentIndent))