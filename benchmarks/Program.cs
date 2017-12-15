using System;
using System.Linq;
using Sprache;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static csparser.Parser;
using static System.Char;

namespace benchmarks
{
    [MemoryDiagnoser]
    public class Program
    {
        [Benchmark]
        public void InternalVersion()
        {
            string identifier(string input)
            {
                input
                    .AsSpan()
                    .Skip(IsWhiteSpace)
                    .TakeOne(IsLetter, out var first)
                    .Take(IsLetterOrDigit, out var rest)
                    .Skip(IsWhiteSpace);

                return $"{first}{rest.AsString()}";
            }

            var id = identifier(" abc123  ");
            Assert.AreEqual("abc123", id);
        }

        [Benchmark]
        public void SpracheVersion() 
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

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Program>();
        }
    }
}
