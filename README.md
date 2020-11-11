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
BenchmarkDotNet=v0.12.1, OS=macOS Catalina 10.15.7 (19H15) [Darwin 19.6.0]
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
```

### Identifier

|        Method |       Mean |   Error |  StdDev | Ratio | RatioSD |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|-------------- |-----------:|--------:|--------:|------:|--------:|-------:|-------:|------:|----------:|
|   SprogSimple |   103.0 ns | 0.53 ns | 0.47 ns |  1.00 |    0.00 | 0.0471 |      - |     - |     296 B |
|   RegexSimple |   235.9 ns | 1.23 ns | 1.15 ns |  2.29 |    0.02 | 0.0675 |      - |     - |     424 B |
| SpracheSimple | 1,819.5 ns | 7.97 ns | 6.66 ns | 17.67 |    0.10 | 0.9232 | 0.0038 |     - |    5792 B |


### XML

|     Method |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|----------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|------:|----------:|
|   SprogXml |   4.654 μs | 0.0197 μs | 0.0175 μs |  1.00 |    0.00 |  1.4038 | 0.0076 |     - |   8.62 KB |
| SpracheXml | 126.919 μs | 0.5976 μs | 0.4990 μs | 27.27 |    0.15 | 51.7578 | 1.7090 |     - | 318.43 KB |


## Roadmap
- Improved samples and documentation
- More test coverage
