# Sprog 
![Build Status](https://wivuu.visualstudio.com/_apis/public/build/definitions/1d87258d-f96d-4792-bf63-430c8cea1376/7/badge)

![Nuget](https://img.shields.io/nuget/v/wivuu.sprog.svg)

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
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.248)
Processor=Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), ProcessorCount=8
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0

        Method |         Mean |       Error |      StdDev |
-------------- |-------------:|------------:|------------:|
-- Identifier
   SprogSimple |     144.8 ns |   0.7640 ns |   0.6380 ns |
   RegexSimple |     366.0 ns |   3.7773 ns |   3.5333 ns |
 SpracheSimple |   1,893.6 ns |  11.1901 ns |   9.9197 ns |

-- XML
      SprogXml |   5,093.0 ns |  13.3911 ns |  11.8708 ns |
    SpracheXml | 126,225.1 ns | 398.7293 ns | 332.9570 ns |
```

## Roadmap
- Improved samples and documentation
- More test coverage
