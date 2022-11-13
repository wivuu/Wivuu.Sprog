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
Preliminary results are promising, with performance nearly 30x faster than [Sprache](https://github.com/sprache/Sprache/) in a naive XML parsing benchmark, using far less memory and GC usage thanks to `Sprog`s reliance on the stack rather than heap allocation.

```
BenchmarkDotNet=v0.13.2, OS=macOS 13.0.1 (22A400) [Darwin 22.1.0]
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 6 logical and 6 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
```

### Identifier

|               Method |        Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|--------------------- |------------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
|          SprogSimple |    85.69 ns |  0.565 ns |  0.501 ns |  1.00 |    0.00 | 0.0063 |      - |      40 B |        1.00 |
|          RegexSimple |   207.36 ns |  1.439 ns |  1.202 ns |  2.42 |    0.02 | 0.0675 |      - |     424 B |       10.60 |
| RegexSourceGenerator |   198.15 ns |  3.678 ns |  5.505 ns |  2.36 |    0.06 | 0.0675 |      - |     424 B |       10.60 |
|        SpracheSimple | 1,681.40 ns | 25.148 ns | 23.523 ns | 19.64 |    0.28 | 0.9155 | 0.0038 |    5744 B |      143.60 |


### XML

|     Method |       Mean |     Error |    StdDev | Ratio | RatioSD |    Gen0 |   Gen1 | Allocated | Alloc Ratio |
|----------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
|   SprogXml |   3.721 μs | 0.0340 μs | 0.0284 μs |  1.00 |    0.00 |  0.3738 |      - |    2.3 KB |        1.00 |
| SpracheXml | 120.923 μs | 0.8570 μs | 0.7597 μs | 32.49 |    0.38 | 51.2695 | 1.7090 | 314.19 KB |      136.33 |


## Roadmap
- Improved samples and documentation
- More test coverage
