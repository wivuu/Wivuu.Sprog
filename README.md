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
| SprogSimple          |  38.51 ns | 0.129 ns | 0.120 ns |  1.00 |    0.00 | 0.0048 |      - |      40 B |        1.00 |
| RegexSimple          | 118.65 ns | 0.294 ns | 0.261 ns |  3.08 |    0.01 | 0.0505 |      - |     424 B |       10.60 |
| RegexSourceGenerator | 114.70 ns | 0.215 ns | 0.190 ns |  2.98 |    0.01 | 0.0507 |      - |     424 B |       10.60 |
| SpracheSimple        | 857.36 ns | 1.382 ns | 1.154 ns | 22.27 |    0.08 | 0.6866 | 0.0019 |    5744 B |      143.60 |


### XML

|     Method |       Mean |     Error |    StdDev | Ratio | RatioSD |    Gen0 |   Gen1 | Allocated | Alloc Ratio |
|----------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
| SprogXml   |  1.555 μs | 0.0136 μs | 0.0127 μs |  1.00 |    0.00 |  0.2804 |      - |    2.3 KB |        1.00 |
| SpracheXml | 56.573 μs | 0.4347 μs | 0.4066 μs | 36.38 |    0.43 | 38.4521 | 1.2817 | 314.19 KB |      136.33 |



## Roadmap
- Improved samples and documentation
- More test coverage
