using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Char;
using CharSpan = csparser.ParserContext;

namespace csparser
{
    public static class XmlParserContext
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
                 .Assert(endName == name, $"End tag {name} not matched")
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

            try 
            {
                items = new List<Item>();
                while (NextItem(out Item next))
                    items.Add(next);
            }
            catch (ParseException e)
            {
                throw e.AddDetail(expected: "XML end-tag");
            }

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
                new ParserContext(xml)
                    .Skip(IsWhiteSpace)
                    .ParseNode(out var n);
                
                document = new Document { Root = n };
                return true;
            }
            catch (ParseException)
            {
                document = null;
                return false;
            }
        }
    }

    [TestClass]
    public class TestXmlContext
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
            <li>Item 1</li>
            <li>
                <ul>
                    <li>Item 2.1</li>
                    <li>Item 2.2</li>
                    <li>Item 2.3</li>
                </ul>
            </li>
            <li>Item 3</li>
            <li>Item 4
            <li>Item 5</li>
        </ul>";

        [TestMethod]
        public void TestParseContext()
        {
            Assert.IsTrue(XmlParserContext.TryParse(SourceXml, out var doc));
            Assert.AreEqual(5, doc.Root.Children.Count());
        }

        [TestMethod]
        public void TestBadXml()
        {
            Assert.IsFalse(XmlParserContext.TryParse(BadXml, out var doc));
        }
    }
}