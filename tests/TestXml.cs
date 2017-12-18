using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Char;
using CharSpan = System.ReadOnlySpan<char>;

namespace csparser
{
    public class Document
    {
        public Node Root;

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
            input.SkipOne(c => c == '<')
                 .ParseIdentifier(out name)
                 .Peek(out var nextC)
                 .Let(selfClosing = nextC == '/')
                 .SkipOne(c => c == '>')
                 .Skip(IsWhiteSpace);

        static CharSpan ParseEndTag(this CharSpan input, string name) =>
            input.SkipOne(c => c == '<')
                 .SkipOne(c => c == '/')
                 .ParseIdentifier(out var _)
                 .SkipOne(c => c == '>')
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
                else if (input.StartsWith("<"))
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

            items = new List<Item>();
            while (NextItem(out Item next))
                items.Add(next);

            return input;
        }

        // static CharSpan ParseNode(this CharSpan input, out Node n) =>
        //     input.ParseTag(out var id, out var selfClosing)
        //         .Rest(out var rest)
        //         .Let(selfClosing
        //         ? input.Let(n = new Node { Name = id })
        //         : input.ParseItems(out var children)
        //                .Let(n = new Node { Name = id, Children = children })
        //                .ParseEndTag(id));

        static CharSpan ParseNode(this CharSpan input, out Node n) 
        {
            input = input.ParseTag(out var id, out var selfClosing);
            
            return selfClosing 
                ? input.Let(n = new Node { Name = id })
                : input.ParseItems(out var children)
                       .Let(n = new Node { Name = id, Children = children })
                       .ParseEndTag(id);
        }

        public static bool TryParse(string xml, out Document document)
        {
            xml.AsSpan()
               .Skip(IsWhiteSpace)
               .ParseNode(out var n);
               
            document = new Document { Root = n };
            return true;
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
        </ul>
        ";

        [TestMethod]
        public void TestParse()
        {
            Assert.IsTrue(XmlParser.TryParse(SourceXml, out var doc));
            Assert.AreEqual(5, doc.Root.Children.Count());
        }
    }
}