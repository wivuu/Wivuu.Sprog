# Sprog
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
BenchmarkDotNet=v0.11.5, OS=macOS 10.15 (19A583) [Darwin 19.0.0]
Intel Core i7-6820HQ CPU 2.70GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
```

### Identifier

|        Method |         Mean |        Error |       StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |-------------:|-------------:|-------------:|--------:|------:|------:|----------:|
|   SprogSimple |     182.3 ns |     3.432 ns |     3.043 ns |  0.0899 |     - |     - |     376 B |
|   RegexSimple |     526.4 ns |     6.236 ns |     5.207 ns |  0.1011 |     - |     - |     424 B |
| SpracheSimple |   2,692.3 ns |    46.282 ns |    41.028 ns |  1.3657 |     - |     - |    5712 B |

### XML

|        Method |         Mean |        Error |       StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |-------------:|-------------:|-------------:|--------:|------:|------:|----------:|
|      SprogXml |   6,881.5 ns |    66.527 ns |    58.974 ns |  2.4490 |     - |     - |   10264 B |
|    SpracheXml | 192,068.5 ns | 3,759.052 ns | 6,583.676 ns | 77.1484 |     - |     - |  322832 B |

## Roadmap
- Improved samples and documentation
- More test coverage
