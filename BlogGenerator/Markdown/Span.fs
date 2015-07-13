namespace Markdown

module Span =

    open Syntax.Definitions
    open Helper

    // an active pattern for parsing inline command pattern
    let (|StartsWith|_|) prefix input =
        let rec loop = function
            | p::prefix, r::rest when p = r ->
                loop (prefix, rest)
            | [], rest ->
                Some(rest)
            | _ ->
                None
        loop (prefix, input)

    let rec parseBracketedBody closing acc = function
        | StartsWith closing (rest) ->
            Some(List.rev acc, rest)
        | c::chars ->
            parseBracketedBody closing (c::acc) chars
        | _ ->
            None

    let parseBracketed opening closing = function
        | StartsWith opening chars ->
            parseBracketedBody closing [] chars
        | _ ->
            None

    let (|Delimited|_|) delim = parseBracketed delim delim

    let (|Bracketed|_|) opening closing = parseBracketed opening closing

    /// MarkdownSpan parser
    let rec parseSpans acc chars = seq {
        let emitLiteral = seq {
            if acc <> [] then
                yield acc |> List.rev |> toString |> Literal }
        
        // match inline syntax pattern
        match chars with
            // inline code
            | Delimited ['`'] (body, chars) ->
                yield! emitLiteral
                yield InlineCode(toString body)
                yield! parseSpans [] chars
            
            // strong
            | Delimited ['*'; '*'] (body, chars)
            | Delimited ['*'] (body, chars) ->
                yield! emitLiteral
                yield Strong(parseSpans [] body |> List.ofSeq)
                yield! parseSpans [] chars
            
            // emphasis
            | Delimited ['_'; '_'] (body, chars)
            | Delimited ['_'] (body, chars) ->
                yield! emitLiteral
                yield Emphasis(parseSpans [] body |> List.ofSeq)
                yield! parseSpans [] chars
            
            // hyperlink
            | Bracketed ['['] [']'] (body, chars) ->
                match chars with
                    | Bracketed ['('] [')'] (url, chars) ->
                        yield! emitLiteral
                        yield HyperLink(parseSpans [] body |> List.ofSeq, toString url)
                        yield! parseSpans [] chars
                    | _ ->
                        yield! parseSpans ('[' :: acc) (List.append body (']' :: chars))
            
            // imagelink
            | Bracketed ['!'; '['] [']'] (body, chars) ->
                match chars with
                    | Bracketed ['('] [')'] (url, chars) ->
                        yield! emitLiteral
                        yield ImageLink(toString body, toString url)
                        yield! parseSpans [] chars
                    | _ ->
                        yield! parseSpans ('[' :: ('!' :: acc)) (List.append body (']' :: chars))

            // hard break
            | StartsWith [' '; ' '; '\\'; 'r'; '\\'; 'n'] chars ->
                yield! emitLiteral
                yield HardBreak
                yield! parseSpans [] chars
            
            // not command related letter
            | c :: chars ->
                yield! parseSpans (c :: acc) chars
            
            // empty
            | [] ->
                yield! emitLiteral }
