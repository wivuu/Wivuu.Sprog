using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Char;
using CharSpan = System.ReadOnlySpan<char>;

namespace Wivuu.Sprog
{
    public class Document
    {
        public Node Root;

        public ParserError Error;

        public override string ToString() => Root.ToString();
    }

    public class Item { }

    public class Content : Item
    {
        public string Text;

        public override string ToString() => Text;
    }

    public class Node : Item
    {
        public string Name;
        public IEnumerable<Item> Children;

        public override string ToString()
        {
            if (Children != null)
                return string.Format("<{0}>", Name) +
                    Children.Aggregate("", (s, c) => s + c) +
                    string.Format("</{0}>", Name);
            return string.Format("<{0}/>", Name);
        }
    }

    public static class XmlParser
    {
        static CharSpan ParseIdentifier(this CharSpan input, out string identifier) =>
            input.Skip(IsWhiteSpace)
                 .TakeOne(IsLetter, out var first)
                 .Take(IsLetterOrDigit, out var rest)
                 .Let(identifier = string.Concat(first, rest))
                 .Skip(IsWhiteSpace);

        static CharSpan ParseTag(this CharSpan input, out string name, out bool selfClosing) =>
            input.SkipOne('<')
                 .ParseIdentifier(out name)
                 .Peek(out var nextC)
                 .Let(selfClosing = nextC == '/')
                 .SkipOne('>')
                 .Skip(IsWhiteSpace);

        static CharSpan ParseEndTag(this CharSpan input, string name) =>
            input.Skip("</")
                 .ParseIdentifier(out var endName)
                 .Assert(endName == name ? null : $"Expected end tag </{name}>")
                 .SkipOne('>')
                 .Skip(IsWhiteSpace);

        static CharSpan ParseItems(this CharSpan input, out List<Item> items)
        {
            bool NextItem(out Item next)
            {
                if (input.StartsWith("</"))
                {
                    next = null;
                    return false;
                }
                else if (input.StartsWith('<'))
                {
                    input = input.ParseNode(out var n);
                    next  = n;
                    return true;
                }
                else
                {
                    input = input.Take(c => c != '<', out var content);
                    next  = new Content { Text = content };
                    return true;
                }
            }

            items = new List<Item>(capacity: 1);
            while (NextItem(out var next))
                items.Add(next);

            return input;
        }

        static CharSpan ParseNode(this CharSpan input, out Node n) =>
            input.ParseTag(out var id, out var selfClosing)
                 .Rest(out var rest)
                 .Let(selfClosing ? rest.Let(n = new Node { Name = id })
                                  : rest.ParseItems(out var children)
                                        .Let(n = new Node { Name = id, Children = children })
                                        .ParseEndTag(id));

        public static bool TryParse(string xml, out Document document)
        {
            try
            {
                xml.AsSpan()
                    .Skip(IsWhiteSpace)
                    .ParseNode(out var n);
               
                document = new Document { Root = n };
                return true;
            }
            catch (ParserException e)
            {
                document = new Document
                { 
                    Error = new ParserError(e.Remaining, e.Assertion).CalculateLineAndCol(xml) 
                };

                return false;
            }
        }
    }

    [TestClass]
    public class TestXml
    {
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
        </ul>";

        const string BadXml = @"
        <ul>
            <li>Item 4</lli>
        </ul>";

        [TestMethod]
        public void TestParseRaw()
        {
            Assert.IsTrue(XmlParser.TryParse(SourceXml, out var doc));
            Assert.AreEqual(5, doc.Root.Children.Count());
        }

        [TestMethod]
        public void TestBadXml()
        {
            Assert.IsFalse(XmlParser.TryParse(BadXml, out var doc));
        }
    }
}