## An HTML page created by my [static blog generator](https://github.com/WangYuanCI/PersonalBlogGenerator)

In this page, I'll test all the basic Markdown features.

### Strong, emphasis and inline code

**important `code`** and _emphasized_

### Linkage

[Markdown linkage] (*command*) should be written [This way](http://cn.bing.com)

### Image

![A cat you may see](..\img\TestImage.jpg)

This is an image of a cat for test.

This is an invalid ![image linkage] (command).

### Hardbreak

Hello  \r\nWorld  \r\n!!!

### Code block

F# is a _functional-first_ programming language, which looks like:

    let meg = "world"
    printfn "hello %s!" msg

This sample code snippet above prints `hello world!` in your **Console**.

### Quotation block

> This is a _hand-written_ quotation block test
which is a paragraph consists of two lines but with
only a `> ` at the beginning of the first line.

> The second quotation test contains F# code block:
> 
>     printfn "Hello from quoted code!"
> 
> You should expected code block effects inside a quotation block

> The 3rd paragraph is an error-coded quotation
> test which consists of 4 lines but not all of
them start with the specific quote-pattern, namely
> the "> " prefix
