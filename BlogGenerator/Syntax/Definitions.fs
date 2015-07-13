/// Syntax : the definitions of the document structure
namespace Syntax

/// wrapper
module Definitions =
    
    // document syntax tree recursive definition
    type Document = list<DocBlock>

    and DocBlock =
        | Heading of int * DocSpans // level * content
        | Paragraph of DocSpans
        | CodeBlock of list<string>
        | QuotedBlock of list<DocBlock>

    and DocSpans = list<DocSpan>

    and DocSpan =
        | Literal of string
        | InlineCode of string
        | Strong of DocSpans
        | Emphasis of DocSpans
        | HyperLink of DocSpans * string // title * url
        | ImageLink of string * string // title * url
        | HardBreak

