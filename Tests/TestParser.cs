using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Char;

namespace Wivuu.Sprog
{
    public static class ParserExtensions
    {
        public static Parser TakeIdentifier(this Parser input, out string identifier) =>
            input.Skip(IsWhiteSpace)
                 .Take(IsLetter, out char first)
                 .Take(IsLetterOrDigit, out string rest)
                 .Let(identifier = $"{first}{rest}");
    }

    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void TestParserContinuators()
        {
            var remaining = new Parser(" This is a test ");

            while (remaining.Length > 0) 
            {
                remaining = remaining
                    .TakeIdentifier(out var identifier)
                    .Skip(IsWhiteSpace);

                Console.WriteLine(identifier);
            }
        }

        [TestMethod]
        public void TestParseIdents()
        {
            var remaining = 
                new Parser(@"1  B3his is some string  ")
                .SkipOne(IsDigit)
                .Skip(IsWhiteSpace)
                .Peek(out var c)
                .TakeIdentifier(out var ident)
                .Skip(IsWhiteSpace);

            Console.WriteLine($"     Char: `{c}`");
            Console.WriteLine($"    Ident: `{ident}`");
            Console.WriteLine($"Remaining: `{remaining.ToString()}`");
        }

        [TestMethod]
        public void TestDeclaration()
        {
            var remaining = 
                new Parser("Test")
                .Declare(out var fst, "FST");

            Assert.AreEqual("FST", fst);
        }
    }
}