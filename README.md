# Sprog
![Nuget](https://img.shields.io/nuget/v/wivuu.sprog.svg)
![Tests](https://github.com/wivuu/Wivuu.Sprog/actions/workflows/actions-ci.yml/badge.svg
)

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

## NetWasm

The package includes a `netwasm0.1` build, so Sprog can be used in applications compiled with
[NetWasm](https://github.com/zion-sati/NetWasm). Building Sprog from source requires the .NET 10
SDK (10.0.303 or newer); the NetWasm SDK is restored from NuGet.

## Performance
Preliminary results are promising, with performance over 40x faster than [Sprache](https://github.com/sprache/Sprache/) in a naive XML parsing benchmark, using far less memory and GC usage thanks to `Sprog`s reliance on the stack rather than heap allocation.

```
BenchmarkDotNet v0.15.8, macOS 27.0.1 (26A434) [Darwin 27.0.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
```

### Identifier

| Method               | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------------- |----------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| SprogSimple          |  36.40 ns | 0.202 ns | 0.179 ns |  1.00 |    0.01 | 0.0048 |      - |      40 B |        1.00 |
| RegexSimple          | 105.13 ns | 2.071 ns | 2.127 ns |  2.89 |    0.06 | 0.0507 |      - |     424 B |       10.60 |
| RegexSourceGenerator |  94.77 ns | 1.580 ns | 1.478 ns |  2.60 |    0.04 | 0.0507 |      - |     424 B |       10.60 |
| SpracheSimple        | 764.57 ns | 3.255 ns | 2.541 ns | 21.00 |    0.12 | 0.6647 | 0.0029 |    5560 B |      139.00 |


### XML

| Method     | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|----------- |----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
| SprogXml   |  1.133 μs | 0.0055 μs | 0.0051 μs |  1.00 |    0.01 |  0.2861 | 0.0019 |   2.35 KB |        1.00 |
| SpracheXml | 47.428 μs | 0.2005 μs | 0.1777 μs | 41.86 |    0.24 | 34.8511 | 1.0986 | 284.82 KB |      121.12 |


### JSON

| Method         | Mean     | Error     | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |---------:|----------:|----------:|------:|-------:|-------:|----------:|------------:|
| SprogJson      | 2.983 μs | 0.0096 μs | 0.0085 μs |  1.00 | 0.6523 | 0.0076 |   5.34 KB |        1.00 |
| SystemTextJson | 3.349 μs | 0.0102 μs | 0.0096 μs |  1.12 | 0.1793 |      - |   1.49 KB |        0.28 |
| NewtonsoftJson | 5.642 μs | 0.0294 μs | 0.0260 μs |  1.89 | 1.6556 | 0.0610 |  13.56 KB |        2.54 |



## Roadmap
- Improved samples and documentation
- More test coverage
