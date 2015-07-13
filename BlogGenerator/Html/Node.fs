namespace Html

module Node =

    open Helper
    open System.IO

    // create a HTML node with pair-tag-style
    let genPairTagElem (out : TextWriter) indent tag attr body =
        let attrString =
            [ for k, v in attr -> k + "=\"" + v + "\"" ]
            |> String.concat " "
        
        match indent with
            | MulLine(count) ->
                let spaces = genSpaces count
                out.Write(spaces + "<" + tag + attrString + ">\n")
                body(count)
                out.Write(spaces + "</" + tag + ">\n")
            | SinLine(count) ->
                let spaces = genSpaces count
                out.Write(spaces + "<" + tag + attrString + ">")
                body(count)
                out.Write("</" + tag + ">\n")
            | Inline ->
                out.Write("<" + tag + attrString + ">")
                body(0)
                out.Write("</" + tag + ">")

    // create a HTML node with single-tag-style
    let genSingleTagElem (out : TextWriter) indent tag attr =
        let attrString =
            [ for k, v in attr -> k + "=\"" + v + "\"" ]
            |> String.concat " "

        match indent with
            | MulLine(count) 
            | SinLine(count) ->
                let spaces = genSpaces count
                out.Write(spaces + "<" + tag + attrString + ">\n")
            | Inline ->
                out.Write("<" + tag + attrString + ">")
