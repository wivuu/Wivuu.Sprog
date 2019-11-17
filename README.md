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
BenchmarkDotNet=v0.12.0, OS=macOS 10.15.1 (19B88) [Darwin 19.0.0]
Intel Core i7-6820HQ CPU 2.70GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
```

### Identifier

|        Method |       Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |-----------:|---------:|---------:|------:|--------:|-------:|------:|------:|----------:|
|   SprogSimple |   156.5 ns |  1.78 ns |  1.58 ns |  1.00 |    0.00 | 0.0706 |     - |     - |     296 B |
|   RegexSimple |   539.3 ns |  6.94 ns |  6.49 ns |  3.45 |    0.05 | 0.1011 |     - |     - |     424 B |
| SpracheSimple | 2,777.1 ns | 22.71 ns | 21.24 ns | 17.76 |    0.24 | 1.3657 |     - |     - |    5712 B |


### XML

|     Method |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------- |-----------:|----------:|----------:|------:|--------:|--------:|------:|------:|----------:|
|   SprogXml |   6.373 us | 0.0411 us | 0.0364 us |  1.00 |    0.00 |  2.1057 |     - |     - |   8.62 KB |
| SpracheXml | 190.159 us | 2.4556 us | 2.2970 us | 29.84 |    0.41 | 77.1484 |     - |     - | 315.27 KB |

## Roadmap
- Improved samples and documentation
- More test coverage
