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
        static CharSpan Identifier(this CharSpan input, out string identifier) =>
            input.Skip(IsWhiteSpace)
                 .TakeOne(IsLetter, out var first)
                 .Take(IsLetterOrDigit, out var rest)
                 .Let(identifier = string.Concat(first, rest))
                 .Skip(IsWhiteSpace);

        static CharSpan Tag(this CharSpan input, out string name, out bool selfClosing) =>
            input.SkipOne(c => c == '<')
                 .Identifier(out name)
                 .Peek(out var nextC)
                 .Let(selfClosing = nextC == '/')
                 .SkipOne(c => c == '>')
                 .Skip(IsWhiteSpace);

        static CharSpan EndTag(this CharSpan input, string name) =>
            input.SkipOne(c => c == '<')
                 .SkipOne(c => c == '/')
                 .Identifier(out var _)
                 .SkipOne(c => c == '>')
                 .Skip(IsWhiteSpace);

        static CharSpan Items(this CharSpan input, out List<Item> items)
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
                    input = input.Node(out var n);
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

        static CharSpan Node(this CharSpan input, out Node n) 
        {
            input = input.Tag(out var id, out var selfClosing);
            
            return selfClosing 
                ? input.Let(n = new Node { Name = id })
                : input.Items(out var children)
                       .Let(n = new Node { Name = id, Children = children })
                       .EndTag(id);
        }

        public static bool TryParse(string xml, out Document document)
        {
            xml.AsSpan()
               .Skip(IsWhiteSpace)
               .Node(out var n);
               
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
            <li>Item 2</li>
            <li>Item 3</li>
            <li>Item 4</li>
            <li>Item 5</li>
        </ul>
        ";

        [TestMethod]
        public void TestParse() =>
            Assert.IsTrue(XmlParser.TryParse(SourceXml, out var _));

    }
}