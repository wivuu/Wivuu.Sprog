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

        [TestMethod]
        public void TestTakeMany()
        {
            (bool success, int value) TakeInteger(ref Parser input) => input
                .Take(IsDigit, out string strValue)
                .Declare(out var hasValue, int.TryParse(strValue, out var intValue))
                .Skip(IsWhiteSpace)
                .Rest(out var rest)
                .Let(hasValue ? input = rest : input)
                .Return(hasValue ? (true, intValue) : (false, 0));

            new Parser("0 5 6").TakeMany(TakeInteger, out var ints);

            Assert.AreEqual(3, ints.Count);
            Assert.AreEqual(0, ints[0]);
            Assert.AreEqual(5, ints[1]);
            Assert.AreEqual(6, ints[2]);
        }
    }
}