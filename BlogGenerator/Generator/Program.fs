/// Executive
open System.IO
open Markdown.Interface
open Html.Interface
open Html.Helper

let contentIndent = 4
let contentHolePat = @"<!--\$\{Content\}-->"

let postItemIndent = 5
let postItemHolePat = @"<!--\$\{PostItem\}-->"

let entryItemIndent = 5
let entryItemHolePat = @"<!--\$\{EntryItem\}-->"

let moreItemIndent = 5
let moreItemHolePat = @"<!--\$\{MoreItem\}-->"

let getCmdParam switch count (input : string[]) =
    let pos = input |> Array.findIndex (fun str -> str.Contains(switch))
    Array.sub input (pos + 1) count

let getMarkdown dir =
    Directory.GetFiles(dir, "*.md")

let getFilename path =
    Path.GetFileNameWithoutExtension path

let createPage tmpltPath inPath outPath =
    let contentHTML = parseMarkdownFile inPath |> genHTML contentIndent
    let templateHTML = loadTemplate tmpltPath
    let decoder = System.Text.Encoding.UTF8
    use output = File.OpenWrite(outPath)

    injectHTML templateHTML contentHolePat contentHTML
        |> List.iter (fun i ->
            let buff = decoder.GetBytes(i + "\n")
            output.Write(buff, 0, buff.Length))

let createContents tmpltPath outDir =
    let contentsHTML = 
        Directory.GetFiles(outDir, "*.html") |> genHTMLContents contentIndent
    let templateHTML = loadTemplate tmpltPath 
    let decoder = System.Text.Encoding.UTF8
    use output = File.OpenWrite(outDir + "\\Contents.html")

    let Contents = 
        genSpaces(contentIndent) +
        "<p>Note that our contents are not actually organized in certain \
        order. Therefore, you cannot expect to find the <strong>latest</strong> \
        post or things like that. It is certainly to be improved later .</p>\n" +
        contentsHTML

    injectHTML templateHTML contentHolePat Contents
        |> List.iter (fun i ->
            let buff = decoder.GetBytes(i + "\n")
            output.Write(buff, 0, buff.Length))

[<EntryPoint>]
let main argv = 
    
    let inDir = (argv |> getCmdParam "-in" 1).[0]
    let outDir = (argv |> getCmdParam "-out" 1).[0]
    let templatePath = (argv |> getCmdParam "-t" 1).[0]

//    // for test
//    let inDir = @"..\..\..\..\testsrc"
//    let outDir = @"..\..\..\..\tjanblog\tests"
//    let templatePath = @"..\..\..\..\tjanblog\Template.html"

    Directory.GetFiles(outDir, "*.html")
        |> Array.iter (fun i ->
            File.Delete(i))

    getMarkdown inDir
        |> Array.iter (fun i ->
            let outPath = outDir + "\\" + getFilename(i) + ".html"
            createPage templatePath i outPath)

    createContents templatePath outDir

    0 // return an integer exit code
