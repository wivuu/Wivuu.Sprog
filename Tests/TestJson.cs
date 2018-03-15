using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wivuu.Sprog;
using static Wivuu.Sprog.Utilities;
using static System.Char;

namespace Tests
{
    public abstract class JsonValue {}

    public class JsonDocument : JsonValue
    {
        public JsonValue Root { get; }

        public ParserError Error { get; }

        public JsonDocument(ParserError error)
        {
            this.Error = error;
        }

        public JsonDocument(JsonValue root)
        {
            this.Root = root;
        }

        public override string ToString() => Root.ToString();
    }

    public class JsonObject : JsonValue
    {
        public IReadOnlyList<(string, JsonValue)> Properties { get; }

        internal JsonObject(List<(string, JsonValue)> properties)
        {
            Properties = properties;
        }

        public override string ToString()
        {
            string FormatProperty((string Key, JsonValue Value) pair) =>
                $"\"{pair.Key}\": {(pair.Value?.ToString() ?? "null")},";

            return "{ " + Properties.Aggregate("", (s, c) => s + FormatProperty(c)).TrimEnd(',') + " }";
        }
    }

    public class JsonArray : JsonValue
    {
        public IReadOnlyList<JsonValue> Values { get; }

        public JsonArray(IReadOnlyList<JsonValue> values)
        {
            Values = values;
        }

        public override string ToString() =>
            "[ " + Values.Aggregate("", (s, c) => s + c + ",").TrimEnd(',') + " ]";
    }

    public class JsonLiteral : JsonValue
    {
        public object Value { get; }

        public JsonLiteral(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            switch (Value)
            {
                case string stringValue: return $"\"{stringValue}\"";
                case null:               return "null";
                default:                 return Value.ToString();
            }
        }
    }

    public static class SprogJsonParser
    {
        public static Parser ParseNumber(this Parser input, out decimal decimalValue) =>
            input.Skip('-', out var isNegative)
                 .Take(Or(IsDigit, c => c == '.' || c == 'e' || c == 'E' || c == '+' || c == '-'), out string decimalString)
                 .Assert(decimal.TryParse(decimalString, out var value) ? null : "Malformed JSON")
                 .Let(decimalValue = isNegative ? (value * -1) : value);

        public static Parser ParseString(this Parser input, out string stringValue)
        {
            var escaped = false;
            bool StringSequence(char c)
            {
                if (c == '\\' && !escaped)
                    escaped = true;
                else if (c == '"')
                {
                    if (escaped)
                    {
                        escaped = false;
                        return true;
                    }
                    else
                        return false;
                }

                return true;
            }

            return input.Skip('"', out var hasOpenQuote)
                 .Assert(hasOpenQuote ? null : "Open quote missing")
                 .Take(StringSequence, out string value)
                 .Skip('"', out var hasCloseQuote)
                 .Assert(hasOpenQuote ? null : "Found EOF instead of end quote")
                 .Let(stringValue = value);
        }

        public static Parser ParseObject(this Parser input, out JsonValue obj)
        {
            (bool success, (string, JsonValue) value) TakeProperty(ref Parser rest) =>
                rest.Skip(IsWhiteSpace)
                    .SkipOne(',')
                    .Skip(IsWhiteSpace)
                    .Rest(out rest)
                    .Peek(out var quote)
                    .Return(quote == '"'
                        ? rest.ParseString(out var propertyName)
                              .Assert(propertyName?.Length > 0
                                      ? null : "Object property name not valid")
                              .Skip(IsWhiteSpace)
                              .Skip(':', out var hasColon)
                              .Assert(hasColon ? null : "Expected ':'")
                              // Parse property value
                              .ParseJson(out var propertyValue)
                              // Skip to either close object or comma
                              .Skip(IsWhiteSpace)
                              .Rest(out rest)
                              .Skip(',', out var isNextObj)
                              .Skip('}', out var isEndObj)
                              .Assert(isNextObj || isEndObj ? null : "Expected ',' or '}'")
                              .Return((true, (propertyName, propertyValue)))
                        : (false, default)
                    );

            return input.TakeMany(TakeProperty, out var items)
                        .Skip(IsWhiteSpace)
                        .Skip('}', out var hasEndObj)
                        .Assert(hasEndObj ? null : "Expected '}'")
                        .Let(obj = new JsonObject(items));
        }

        public static Parser ParseArray(this Parser input, out JsonValue array)
        {
            (bool success, JsonValue value) TakeItem(ref Parser rest) =>
                rest
                    .Skip(IsWhiteSpace)
                    .SkipOne(',')
                    .Skip(IsWhiteSpace)
                    .Rest(out rest)
                    .Skip(']', out var isEnded)
                    .Return(!isEnded
                        ? rest.ParseValue(out var value)
                              .Rest(out rest)
                              .Return((true, value))
                        : (false, default)
                    );

            return input.TakeMany(TakeItem, out List<JsonValue> items)
                        .Let(array = new JsonArray(items))
                        .Skip(']', out var hasEndArr)
                        .Assert(hasEndArr ? null : "Expected ']'");
        }


        public static Parser ParseValue(this Parser input, out JsonValue value)
        {
            if (input.Skip("null", out var isNull).If(isNull, out input))
            {
                value = null;
                return input;
            }
            else if (input.Skip("true", out var isTrue).If(isTrue, out input))
            {
                value = new JsonLiteral(true);
                return input;
            }
            else if (input.Skip("false", out var isFalse).If(isFalse, out input))
            {
                value = new JsonLiteral(false);
                return input;
            }
            else if (input.StartsWith('"'))
                return input.ParseString(out var stringValue)
                            .Let(value = new JsonLiteral(stringValue));
            else
                return input.ParseNumber(out var numValue)
                            .Let(value = new JsonLiteral(numValue));
        }

        public static Parser ParseJson(this Parser input, out JsonValue root) =>
            input.Skip(IsWhiteSpace)
                 .Rest(out input)
                 .Peek(out var c)
                 .Let(
                     c == '{' ? input.SkipOne().ParseObject(out root) :
                     c == '[' ? input.SkipOne().ParseArray(out root) :
                                input.ParseValue(out root)
                 );

        public static bool TryParse(string json, out JsonDocument document)
        {
            try
            {
                new Parser(json)
                    .ParseJson(out var n);

                document = new JsonDocument(root: n);
                return true;
            }
            catch (ParserException e)
            {
                document = new JsonDocument(error: new ParserError(e, json));
                return false;
            }
        }
    }

    [TestClass]
    public class TestJson
    {
        readonly string[] GoodJson =
        {
            @"{
                ""PositiveNumber"" : 512.52,
                ""NegativeNumber"": -100,
                ""Array"": [ ""Item1"", ""Item2"" ],
                ""Null"": null,
                ""Obj"": { ""Prop1"": ""Value"" }
            }"
        };

        readonly string[] BadJson =
        {
            // Line 5, missing end ']'
            @"{
                ""PositiveNumber"": 512.52,
                ""NegativeNumber"": -100,
                ""Array"": [ ""Item1"", ""Item2"",
                ""Null"": null,
                ""Obj"": { ""Prop1"": ""Value"" }
            }"
        };

        [TestMethod]
        public void TestToString()
        {
            var doc = new JsonObject(new List<(string, JsonValue)>
            {
                ("TestValue", new JsonLiteral(5)),
                ("TestArray", new JsonArray(new List<JsonValue>
                {
                    new JsonLiteral(1),
                    new JsonLiteral(2),
                    new JsonLiteral(3),
                    new JsonLiteral(4),
                })),
                ("NextValue", null)
            });

            var value = doc.ToString();
        }

        [TestMethod]
        public void TestGoodJson()
        {
            JsonDocument doc;
            foreach (var good in GoodJson)
                Assert.IsTrue(SprogJsonParser.TryParse(good, out doc));
        }

        [TestMethod]
        public void TestBadJson()
        {
            JsonDocument doc;
            foreach (var bad in BadJson)
                Assert.IsFalse(SprogJsonParser.TryParse(bad, out doc));
        }
    }
}