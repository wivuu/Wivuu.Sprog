using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using Wivuu.Sprog;
using static System.Char;

BenchmarkRunner.Run<benchmarks.Simple>();
BenchmarkRunner.Run<benchmarks.Xml>();
BenchmarkRunner.Run<benchmarks.Json>();

namespace benchmarks
{
    [MemoryDiagnoser]
    public partial class Simple
    {
        #region Simple

        [Benchmark(Baseline = true)]
        public void SprogSimple()
        {
            static Parser TakeIdentifier(string input, out string _id) =>
                new Parser(input)
                    .Skip(IsWhiteSpace)
                    .Take(IsLetter, out char first)
                    .Take(IsLetterOrDigit, out ReadOnlySpan<char> rest)
                    .Skip(IsWhiteSpace)
                    .Let(_id = first.Concat(rest));

            TakeIdentifier(" abc123  ", out var id);
            Assert.AreEqual("abc123", id);
        }

        static Regex SimplePattern = new Regex(@"\s*(?<ident>[a-zA-Z][a-zA-Z0-9]*)\s*", RegexOptions.Compiled);

        [Benchmark]
        public void RegexSimple()
        {
            static string TakeIdentifier(string input) =>
                SimplePattern.Match(input).Groups["ident"].Value;

            var id = TakeIdentifier(" abc123  ");
            Assert.AreEqual("abc123", id);
        }

        [GeneratedRegex(@"\s*(?<ident>[a-zA-Z][a-zA-Z0-9]*)\s*", RegexOptions.CultureInvariant)]
        public static partial Regex SourceGenPattern();

        [Benchmark]
        public void RegexSourceGenerator()
        {
            static string TakeIdentifier(string input) =>
                SourceGenPattern().Match(input).Groups["ident"].Value;

            var id = TakeIdentifier(" abc123  ");
            Assert.AreEqual("abc123", id);
        }

        [Benchmark]
        public void SpracheSimple()
        {
            var identifier =
                from leading in Sprache.Parse.WhiteSpace.Many()
                from first in Sprache.Parse.Letter.Once()
                from rest in Sprache.Parse.LetterOrDigit.Many()
                from trailing in Sprache.Parse.WhiteSpace.Many()
                select new string(first.Concat(rest).ToArray());

            var id = identifier.Parse(" abc123  ");
            Assert.AreEqual("abc123", id);
        }

        #endregion

    }

    [MemoryDiagnoser]
    public class Xml
    {
        #region Xml

        const string SourceXml = """
        <ul>
            <li>Item 1</li>
            <li>
                <ul>
                    <li>Item 2.1</li>
                    <li>Item 2.2</li>
                    <li>Item 2.3</li>
                </ul>
            </li>
            <li>Item 3</li>
            <li>Item 4</li>
            <li>Item 5</li>
        </ul>
        """;

        [Benchmark(Baseline = true)]
        public void SprogXml() =>
            SprogXmlParser.TryParse(SourceXml, out var _);

        [Benchmark]
        public void SpracheXml() =>
            SpracheXmlParser.Document.Parse(SourceXml);

        #endregion
    }

    [MemoryDiagnoser]
    public class Json
    {
        #region Json

        readonly string GoodJson =
            """
            {
                "type": "FeatureCollection",
                "features": [
                    { "type": "Feature", "properties": { "MAPBLKLOT": "0001001", "BLKLOT": "0001001", "BLOCK_NUM": "0001", "LOT_NUM": "001", "FROM_ST": "0", "TO_ST": "0", "STREET": "UNKNOWN", "ST_TYPE": null, "ODD_EVEN": "E" }, "geometry": { "type": "Polygon", "coordinates": [ [ [ -122.422003528252475, 37.808480096967251, 0.0 ], [ -122.422076013325281, 37.808835019815085, 0.0 ], [ -122.421102174348633, 37.808803534992904, 0.0 ], [ -122.421062569067274, 37.808601056818148, 0.0 ], [ -122.422003528252475, 37.808480096967251, 0.0 ] ] ] } }
                ]
            }
            """;

        [Benchmark(Baseline = true)]
        public void SprogJson() =>
            SprogJsonParser.TryParse(GoodJson, out var _);

        [Benchmark]
        public void SystemTextJson() =>
            System.Text.Json.JsonSerializer.Deserialize<JsonElement>(GoodJson);

        [Benchmark]
        public void NewtonsoftJson() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(GoodJson);

        #endregion
    }
}