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

BenchmarkDotNet=v0.11.5, OS=macOS Mojave 10.14.4 (18E226) [Darwin 18.5.0]
Intel Core i7-6820HQ CPU 2.70GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.105
  [Host]     : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT

### Identifier

|        Method |         Mean |        Error |       StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |-------------:|-------------:|-------------:|--------:|------:|------:|----------:|
|   SprogSimple |     186.4 ns |     3.005 ns |     2.811 ns |  0.0932 |     - |     - |     392 B |
|   RegexSimple |     561.9 ns |     7.693 ns |     7.196 ns |  0.1011 |     - |     - |     424 B |
| SpracheSimple |   2,908.5 ns |    37.716 ns |    35.280 ns |  1.3885 |     - |     - |    5832 B |

### XML

|        Method |         Mean |        Error |       StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |-------------:|-------------:|-------------:|--------:|------:|------:|----------:|
|      SprogXml |   6,819.7 ns |    80.744 ns |    75.528 ns |  2.5406 |     - |     - |   10688 B |
|    SpracheXml | 208,274.6 ns | 1,598.243 ns | 1,416.801 ns | 78.3691 |     - |     - |  329128 B |

## Roadmap
- Improved samples and documentation
- More test coverage
