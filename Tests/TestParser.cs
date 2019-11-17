using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Char;
using static Wivuu.Sprog.Utilities;

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
        public void TestConcat()
        {
            ReadOnlySpan<char> span1 = "Test value";
            ReadOnlySpan<char> span2 = "Value2";

            var rhs = span1.Concat('!');
            var lhs = '!'.Concat(span1);
            var twoSpans = span1.Concat(span2);

            Assert.AreEqual("Test value!", rhs);
            Assert.AreEqual("!Test value", lhs);
            Assert.AreEqual("Test valueValue2", twoSpans);
        }

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
                .Return(hasValue ? (true, intValue) : (false, default));

            new Parser("0 5 6").TakeMany(TakeInteger, out var ints);

            Assert.AreEqual(3, ints.Count);
            Assert.AreEqual(0, ints[0]);
            Assert.AreEqual(5, ints[1]);
            Assert.AreEqual(6, ints[2]);
        }

        [TestMethod]
        public void TestNot()
        {
            var result = new Parser(" This is a test ")
                .TakeMany(
                    (ref Parser rest) => rest
                        .Skip(IsWhiteSpace)
                        .Take(Not(IsWhiteSpace), out string word)
                        .Rest(out rest)
                        .Return(word?.Length > 0 
                            ? (true, word) 
                            : (false, default)), 
                    out var words
                )
                .Return(words);

            Assert.AreEqual(4, words.Count);

            foreach (var (actual, expected) in words.Zip(new [] { "This","is","a","test" }, (l,r) => (r, l)))
            {
                Assert.AreEqual(actual, expected);
            }
        }
    }
}