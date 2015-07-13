/// Html : the HTML & CSS code generator
namespace Html

/// Library user interface
module Interface =

    open Helper
    open Generate
    open System.IO
    open System.Text
    open System.Text.RegularExpressions
    
    let genHTML indentCount docList =
        let strBuilder = StringBuilder()
        let buff = new StringWriter(strBuilder)
        let indent = MulLine(indentCount)

        docList |> List.iter (fun it -> genBlock buff indent it)
        strBuilder.ToString()

    let genHTMLContents indentCount pageList =
        let spaces = genSpaces indentCount
        let itemSpaces = genSpaces (indentCount + 1)
        let items =
            pageList
                |> Array.fold (fun state i ->
                    let title = Path.GetFileNameWithoutExtension i
                    let src = Path.GetFileName i
                    let item = sprintf "<li><a href=\"%s\">%s</a></li>\n" src title
                    state + itemSpaces + item) ""
        let contentsHead = spaces + "<h2>Contents</h2>\n" + spaces + "<p><ul>\n"
        let contentsFoot = spaces + "</ul></p>\n"
        contentsHead + items + contentsFoot

    let loadTemplate path =
        let stream = File.OpenText(path)

        seq {
            while (not stream.EndOfStream) do
                yield stream.ReadLine()
        } |> List.ofSeq

    let injectHTML htmlList pat (sub : string) =
        let replacePattern = Regex pat
        htmlList
            |> List.map (fun i -> replacePattern.Replace(i, sub))