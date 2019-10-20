using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using Wivuu.Sprog;
using static System.Char;
using static System.String;

namespace benchmarks
{
    [MemoryDiagnoser]
    public class Simple
    {
        #region Simple

        [Benchmark(Baseline = true)]
        public void SprogSimple()
        {
            static Parser TakeIdentifier(string input, out string _id) =>
                new Parser(input)
                    .Skip(IsWhiteSpace)
                    .Take(IsLetter, out char first)
                    .Take(IsLetterOrDigit, out string rest)
                    .Skip(IsWhiteSpace)
                    .Let(_id = Concat(first, rest));

            TakeIdentifier(" abc123  ", out var id);
            Assert.AreEqual("abc123", id);
        }

        static Regex Pattern = new Regex(@"\s*(?<ident>[a-zA-Z][a-zA-Z0-9]*)\s*", RegexOptions.Compiled);

        [Benchmark]
        public void RegexSimple()
        {
            static string TakeIdentifier(string input) =>
                Pattern.Match(input).Groups["ident"].Value;

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

        const string SourceXml = @"
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
        ";

        [Benchmark(Baseline = true)]
        public void SprogXml() =>
            SprogXmlParser.TryParse(SourceXml, out var _);

        [Benchmark]
        public void SpracheXml() =>
            SpracheXmlParser.Document.Parse(SourceXml);

        #endregion

        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Simple>();
            BenchmarkRunner.Run<Xml>();
        }
    }
}
