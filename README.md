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
BenchmarkDotNet=v0.13.5, OS=macOS 14.0 (23A344) [Darwin 23.0.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK=8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
```

### Identifier

|               Method |        Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|--------------------- |------------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
|          SprogSimple |  42.53 ns | 0.159 ns | 0.148 ns |  1.00 |    0.00 | 0.0048 |      - |      40 B |        1.00 |
|          RegexSimple | 120.62 ns | 1.119 ns | 1.047 ns |  2.84 |    0.02 | 0.0505 |      - |     424 B |       10.60 |
| RegexSourceGenerator | 119.41 ns | 0.434 ns | 0.406 ns |  2.81 |    0.02 | 0.0505 |      - |     424 B |       10.60 |
|        SpracheSimple | 881.47 ns | 3.194 ns | 2.988 ns | 20.73 |    0.11 | 0.6866 | 0.0019 |    5744 B |      143.60 |


### XML

|     Method |       Mean |     Error |    StdDev | Ratio | RatioSD |    Gen0 |   Gen1 | Allocated | Alloc Ratio |
|----------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
|   SprogXml |  1.840 μs | 0.0101 μs | 0.0090 μs |  1.00 |    0.00 |  0.2804 |      - |    2.3 KB |        1.00 |
| SpracheXml | 57.778 μs | 0.6991 μs | 0.6539 μs | 31.43 |    0.30 | 38.4521 | 1.2817 | 314.19 KB |      136.33 |



## Roadmap
- Improved samples and documentation
- More test coverage
