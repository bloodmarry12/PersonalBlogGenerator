namespace Markdown

module Block =
    
    open Syntax.Definitions
    open Helper
    open Span

    // expand the List module for convenience purpose
    // a helper high-order function
    module List =
        let partitionWhile f =
            let rec loop acc = function
                | x :: xs when f x -> loop (x :: acc) xs
                | xs -> List.rev acc, xs
            loop []

    // a helper function
    let notWhite = System.String.IsNullOrWhiteSpace >> not

    let (|PrefixedLines|_|) prefix (lines : list<string>) =
        let prefixed, other =
            lines |> List.partitionWhile (fun line ->
                line.StartsWith prefix)
        let prefixedbody = seq {
            for line in prefixed do
                if notWhite line then
                    yield line.Substring(prefix.Length) }
        Some(prefixedbody |> List.ofSeq, other)

    let (|LineSeperated|_|) lines =
        match List.partitionWhile notWhite lines with
            | par, _ :: rest
            | par, ([] as rest) -> Some(par, rest)

    let rec SharpHeading accCnt (line : string) =
        let content = line |> List.ofSeq
        match content with
            | StartsWith ['#'] rest when accCnt < 6 ->
                SharpHeading (accCnt + 1) (toString rest)
            | StartsWith [' '] rest when accCnt > 0 ->
                match notWhite(toString rest) with
                    | true -> Some(accCnt, rest)
                    | false -> None
            | _ -> None

    let (|Heading|_|) (lines : list<string>) =
        match lines with
            | line :: rest ->
                match SharpHeading 0 line with
                    | Some(level, heading) -> Some(level, heading, rest)
                    | _ -> None
            | _ -> None

    let handleEachLine check proc (lines : list<string>) =
        let checkPassed = lines |> List.filter check
        match checkPassed.Length = lines.Length with
            | true -> Some(lines |> List.map proc)
            | _ -> None

    let checkPrefix prefix (line : string) = line.StartsWith prefix

    let chopPrefix (prefix : string) (line : string) = line.Substring prefix.Length

    let isEachLineQuoted (lines : list<string>) =
        handleEachLine (checkPrefix "> ") (chopPrefix "> ") lines

    let isEachLineNotQuoted (lines : list<string>) =
        handleEachLine (checkPrefix "> " >> not) id lines

    let isFirstLineQuoted (lines : list<string>) =
        match lines with
            | firstLine :: rest ->
                match (checkPrefix "> " firstLine) with
                    | true ->
                        match (isEachLineNotQuoted rest) with
                            | Some(_) -> Some(chopPrefix "> " firstLine :: rest)
                            | _ -> None
                    | _ -> None                    
            | [] -> None

    let isQuoted (lines : list<string>) =
        match isEachLineQuoted lines with
            | Some(xs) -> Some(xs)
            | None ->
                match isFirstLineQuoted lines with
                    | Some(xs) -> Some(xs)
                    | _ -> None 

    /// the block parsing function
    let rec parseBlocks lines = seq {
        match lines with
            // heading
            | Heading (level, heading, lines) ->
                yield Heading(level, parseSpans [] heading |> List.ofSeq)
                yield! parseBlocks lines

            // code block
            | PrefixedLines "\t" (body, lines) when body <> [] ->
                yield CodeBlock(body)
                yield! parseBlocks lines

            | PrefixedLines "    " (body, lines) when body <> [] ->
                yield CodeBlock(body)
                yield! parseBlocks lines
             
            // paragraph
            | LineSeperated (body, lines) when body <> [] ->
                let quotedBody = isQuoted body
                match quotedBody with
                    | Some(quoted) ->
                        let quotedLines = List.append quoted [""]
                        yield QuotedBlock(parseBlocks quotedLines |> List.ofSeq)
                    | None ->
                        let body = String.concat " " body |> List.ofSeq
                        yield Paragraph(parseSpans [] body |> List.ofSeq)
                yield! parseBlocks lines                
            
            // empty line
            | line :: lines when System.String.IsNullOrWhiteSpace(line) ->
                yield! parseBlocks lines
            
            // unidentified block
            | _ ->
                () }

