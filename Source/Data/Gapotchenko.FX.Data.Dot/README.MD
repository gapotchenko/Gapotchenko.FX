# Gapotchenko.FX.Data.Dot

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Dot.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Dot)

The module provides tools for parsing and creating DOT graph description language documents, their lexical and syntactic analysis.

## DotReader and DotWriter

With the help of `DotReader` and `DotWriter` classes provided by `Gapotchenko.FX.Data.Dot` module you can convert a DOT document to a stream of tokens and back.

Let's remove all the comments from a DOT document:

```csharp
var dot = "digraph { } // Comment";

using var textReader = new StringReader(dot);
using var dotReader = DotReader.Create(textReader);

using var textWriter = new StringWriter();
using var dotWriter = DotWriter.Create(textWriter);

while (dotReader.Read())
{
    if (dotReader.TokenType is not DotTokenKind.Comment)
    {
        dotWriter.Write(dotReader.TokenType, dotReader.Value);
    }
}

Console.WriteLine(textWriter);
```

## DOT document model

`DotDocument` class provided by `Gapotchenko.FX.Data.Dot` module is a starting point for document model operations. With its methods `Load` and `Save` you can create a document model from a stream of tokens and perform the contrary.

Here is a way how to load a document and print a graph identifier:

```csharp
var dot = "digraph MyGraph { }";

using var textReader = new StringReader(dot);
using var dotReader = DotReader.Create(textReader);

var dotDocument = DotDocument.Load(dotReader);

Console.WriteLine(dotDocument.Root.Identifier.Value);
```

Among other things, `Gapotchenko.FX.Data.Dot` module implements a visitor pattern and the node walker based on it.

The following example shows how to print all the vertex identifiers inside a DOT document:

```csharp
var dot = "digraph { v }";

using var textReader = new StringReader(dot);
using var dotReader = DotReader.Create(textReader);

var dotDocument = DotDocument.Load(dotReader);

dotDocument.Root.Accept(new IdentifierPrinter());

class IdentifierPrinter : DotDomWalker
{
    public override void VisitDotVertexIdentifierNode(DotVertexIdentifierNode node) =>
        Console.WriteLine(node);
}
```

To make the new documents creation process easier, `Gapotchenko.FX.Data.Dot` module provides the ability to perform automatic document formatting.

Here is an example:

```csharp
var dot = "digraph{v}";

using var textReader = new StringReader(dot);
using var dotReader = DotReader.Create(textReader);

var dotDocument = DotDocument.Load(dotReader);

dotDocument.Root.NormalizeWhitespace();

using var textWriter = new StringWriter();
using var dotWriter = DotWriter.Create(textWriter);

dotDocument.Save(dotWriter);

Console.WriteLine(textWriter);
```

## Usage

`Gapotchenko.FX.Data.Dot` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Dot):

```
PM> Install-Package Gapotchenko.FX.Data.Dot
```