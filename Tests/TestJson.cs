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
        public JsonValue? Root { get; }

        public ParserError? Error { get; }

        public JsonDocument(ParserError error)
        {
            this.Error = error;
        }

        public JsonDocument(JsonValue? root)
        {
            this.Root = root;
        }

        public override string? ToString() => Root?.ToString();
    }

    public class JsonObject : JsonValue
    {
        public IReadOnlyList<(string, JsonValue?)> Properties { get; }

        internal JsonObject(List<(string, JsonValue?)> properties)
        {
            Properties = properties;
        }

        public override string ToString()
        {
            static string FormatProperty((string Key, JsonValue? Value) pair) =>
                $"\"{pair.Key}\": {(pair.Value?.ToString() ?? "null")},";

            return "{ " + Properties.Aggregate("", static (s, c) => s + FormatProperty(c)).TrimEnd(',') + " }";
        }
    }

    public class JsonArray : JsonValue
    {
        public IReadOnlyList<JsonValue?> Values { get; }

        public JsonArray(IReadOnlyList<JsonValue?> values)
        {
            Values = values;
        }

        public override string ToString() =>
            "[ " + Values.Aggregate("", static (s, c) => s + c + ",").TrimEnd(',') + " ]";
    }

    public class JsonLiteral : JsonValue
    {
        public object Value { get; }

        public JsonLiteral(object value)
        {
            Value = value;
        }

        public override string ToString() =>
            Value switch
            {
                string stringValue => $"\"{stringValue}\"",
                null               => "null",
                _                  => Value.ToString()!,
            };
    }

    public static class SprogJsonParser
    {
        public static Parser ParseNumber(this Parser input, out decimal decimalValue) =>
            input.Take(Or(IsDigit, static c => c is '.' or 'e' or 'E' or '+' or '-'), out string decimalString)
                 .Assert(decimal.TryParse(decimalString, out var value) ? null : "Malformed JSON")
                 .Let(decimalValue = value);

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

            return input
                .Skip('"', out var hasOpenQuote)
                .Assert(hasOpenQuote ? null : "Open quote missing")
                .Take(StringSequence, out string value)
                .Skip('"', out var hasCloseQuote)
                .Assert(hasOpenQuote ? null : "Found EOF instead of end quote")
                .Let(stringValue = value);
        }

        public static Parser ParseObject(this Parser input, out JsonValue obj)
        {
            (bool success, (string, JsonValue?) value) TakeProperty(ref Parser rest) =>
                rest.Skip(IsWhiteSpace)
                    .SkipOne(',')
                    .Skip(IsWhiteSpace)
                    .Rest(out rest)
                    .Peek(out var quote)
                    .Return(quote == '"'
                        ? rest.ParseString(out var propertyName)
                              .Assert(propertyName is { Length: > 0 }
                                      ? null : "Object property name not valid")
                              .Skip(IsWhiteSpace)
                              .Skip(':', out var hasColon)
                              .Assert(hasColon ? null : "Expected ':'")
                              .Skip(IsWhiteSpace)
                              // Parse property value
                              .ParseJson(out var propertyValue)
                              // Skip to either close object or comma
                              .Skip(IsWhiteSpace)
                              .Rest(out rest)
                              .Skip(',', out var isNextObj)
                              .Skip('}', out var isEndObj)
                              .Assert(isNextObj || isEndObj ? null : "Expected ',' or '}'")
                              .Return((true, (propertyName!, propertyValue)))
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
            static (bool success, JsonValue? value) TakeItem(ref Parser rest) =>
                rest
                    .Skip(IsWhiteSpace)
                    .SkipOne(',')
                    .Skip(IsWhiteSpace)
                    .Rest(out rest)
                    .Skip(']', out var isEnded)
                    .Return(!isEnded
                        ? rest.ParseJson(out var value)
                              .Rest(out rest)
                              .Return((true, value))
                        : (false, default)
                    );

            return input.TakeMany(TakeItem, out List<JsonValue?> items)
                        .Let(array = new JsonArray(items))
                        .Skip(']', out var hasEndArr)
                        .Assert(hasEndArr ? null : "Expected ']'");
        }

        public static Parser ParseJson(this Parser input, out JsonValue? value)
        {
            if (input.StartsWith('{'))
                return input.SkipOne().ParseObject(out value);
            else if (input.StartsWith('['))
                return input.SkipOne().ParseArray(out value);
            else if (input.Skip("null", out var isNull).If(isNull, out input))
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

        public static bool TryParse(string json, out JsonDocument document)
        {
            try
            {
                new Parser(json)
                    .Skip(IsWhiteSpace)
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
            ""type"": ""FeatureCollection"",
            ""features"": [
                { ""type"": ""Feature"", ""properties"": { ""MAPBLKLOT"": ""0001001"", ""BLKLOT"": ""0001001"", ""BLOCK_NUM"": ""0001"", ""LOT_NUM"": ""001"", ""FROM_ST"": ""0"", ""TO_ST"": ""0"", ""STREET"": ""UNKNOWN"", ""ST_TYPE"": null, ""ODD_EVEN"": ""E"" }, ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [ [ [ -122.422003528252475, 37.808480096967251, 0.0 ], [ -122.422076013325281, 37.808835019815085, 0.0 ], [ -122.421102174348633, 37.808803534992904, 0.0 ], [ -122.421062569067274, 37.808601056818148, 0.0 ], [ -122.422003528252475, 37.808480096967251, 0.0 ] ] ] } }
              ]
            }",
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
            var doc = new JsonObject(new List<(string, JsonValue?)>
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