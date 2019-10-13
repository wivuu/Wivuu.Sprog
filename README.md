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
BenchmarkDotNet=v0.11.5, OS=macOS Mojave 10.14.4 (18E226) [Darwin 18.5.0]
Intel Core i7-6820HQ CPU 2.70GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.105
  [Host]     : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
```

### Identifier

|        Method |         Mean |        Error |       StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |-------------:|-------------:|-------------:|--------:|------:|------:|----------:|
|   SprogSimple |     180.4 ns |     3.616 ns |     4.701 ns |  0.0899 |     - |     - |     376 B |
|   RegexSimple |     536.5 ns |     6.310 ns |     5.593 ns |  0.1011 |     - |     - |     424 B |
| SpracheSimple |   2,710.9 ns |    19.738 ns |    16.482 ns |  1.3657 |     - |     - |    5712 B |

### XML

|        Method |         Mean |        Error |       StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |-------------:|-------------:|-------------:|--------:|------:|------:|----------:|
|      SprogXml |   6,630.5 ns |    69.168 ns |    61.316 ns |  2.4490 |     - |     - |   10264 B |
|    SpracheXml | 185,090.2 ns | 1,568.695 ns | 1,224.734 ns | 77.1484 |     - |     - |  322832 B |

## Roadmap
- Improved samples and documentation
- More test coverage
