# Sprog 
![Build Status](https://wivuu.visualstudio.com/_apis/public/build/definitions/1d87258d-f96d-4792-bf63-430c8cea1376/7/badge)

Sprog (Danish for 'Language') cross-platform string parsing library written for C# 7+ and .NET Core.

## Usage

Parsing a simple identifier

```C#
Parser TakeIdentifier(string input, out string _id) =>
    new Parser(input)
        .Skip(IsWhiteSpace)
        .Take(IsLetter, out char first)
        .Take(IsLetterOrDigit, out string rest)
        .Skip(IsWhiteSpace)
        .Let(_id = Concat(first, rest));

TakeIdentifier(" abc123  ", out var id);
Assert.AreEqual("abc123", id);
```

[Sprog Naive XML parser](./Tests/TestXml.cs)

## Performance
Preliminary results are promising, with performance nearly 32x faster than [Sprache](https://github.com/sprache/Sprache/) in a naive XML parsing benchmark, using far less memory and GC usage thanks to `Sprog`s reliance on the stack rather than heap allocation.

```
BenchmarkDotNet=v0.10.11, OS=macOS 10.13.2 (17C88) [Darwin 17.3.0]
Processor=Intel Core i7-6820HQ CPU 2.70GHz (Skylake), ProcessorCount=8
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.0.0), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.0.0), 64bit RyuJIT

        Method |         Mean |        Error |       StdDev |
-------------- |-------------:|-------------:|-------------:|
-- Identifier
   SprogSimple |     193.2 ns |     1.736 ns |     1.624 ns |
   RegexSimple |     686.3 ns |    13.370 ns |    18.300 ns |
 SpracheSimple |   2,794.5 ns |    36.709 ns |    32.542 ns |

 -- XML
      SprogXml |   7,385.3 ns |    93.462 ns |    87.424 ns |
    SpracheXml | 202,844.1 ns | 2,601.748 ns | 2,433.676 ns |
```

## Roadmap
- Improved samples and documentation
- More test coverage

**Note**: This package will be in pre-release at least as long as Microsoft's `System.Memory` is in pre-release.