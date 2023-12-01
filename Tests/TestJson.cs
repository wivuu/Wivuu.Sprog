using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wivuu.Sprog;
using static System.Char;
using System.Buffers;

namespace Tests
{
    public abstract class JsonValue;

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
                $"\"{pair.Key}\": {pair.Value?.ToString() ?? "null"},";

            return "{ " + Properties.Aggregate("", static (s, c) => s + FormatProperty(c)).TrimEnd(',') + " }";
        }
    }

    public class JsonArray(IReadOnlyList<JsonValue?> values) : JsonValue
    {
        public IReadOnlyList<JsonValue?> Values { get; } = values;

        public override string ToString() =>
            "[ " + Values.Aggregate("", static (s, c) => s + c + ",").TrimEnd(',') + " ]";
    }

    public class JsonLiteral(object? value) : JsonValue
    {
        public object? Value { get; } = value;

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
        static readonly SearchValues<char> IsDigitOrExponent = SearchValues.Create("0123456789.eE+-");
        public static Parser ParseNumber(this Parser input, out double decimalValue) =>
            input.Take(IsDigitOrExponent.Contains, out string decimalString)
                 .Assert(double.TryParse(decimalString, out var value), "Malformed JSON")
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
                .Assert(hasOpenQuote, "Open quote missing")
                .Take(StringSequence, out stringValue)
                .Skip('"', out var hasCloseQuote)
                .Assert(hasOpenQuote, "Found EOF instead of end quote");
        }

        public static Parser ParseObject(this Parser input, out JsonValue obj)
        {
            static (bool success, (string, JsonValue?) value) TakeProperty(ref Parser rest) =>
                rest.Skip(IsWhiteSpace)
                    .SkipOne(',')
                    .Skip(IsWhiteSpace)
                    .Rest(out rest)
                    .Peek(out var quote)
                    .Return(quote == '"'
                        ? rest.ParseString(out var propertyName)
                              .Assert(propertyName.Length > 0, "Object property name not valid")
                              .Skip(IsWhiteSpace)
                              .Skip(':', out var hasColon)
                              .Assert(hasColon, "Expected ':'")
                              .Skip(IsWhiteSpace)
                              // Parse property value
                              .ParseJson(out var propertyValue)
                              // Skip to either close object or comma
                              .Skip(IsWhiteSpace)
                              .Rest(out rest)
                              .Skip(',', out var isNextObj)
                              .Skip('}', out var isEndObj)
                              .Assert(isNextObj || isEndObj, "Expected ',' or '}'")
                              .Return((true, (propertyName, propertyValue)))
                        : (false, default)
                    );

            return input.TakeMany(TakeProperty, out var items)
                        .Skip(IsWhiteSpace)
                        .Skip('}', out var hasEndObj)
                        .Assert(hasEndObj, "Expected '}'")
                        .Let(obj = new JsonObject(items));
        }

        public static Parser ParseArray(this Parser input, out JsonValue array)
        {
            static (bool success, JsonValue? value) TakeItem(ref Parser rest) =>
                rest.Skip(IsWhiteSpace)
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
                        .Assert(hasEndArr, "Expected ']'");
        }

        public static Parser ParseJson(this Parser input, out JsonValue? value)
        {
            if (input.StartsWith('{'))
                return input.SkipOne().ParseObject(out value);
            else if (input.StartsWith('['))
                return input.SkipOne().ParseArray(out value);
            else if (input.StartsWith("null"))
            {
                value = null;
                return input.Buffer[4..];
            }
            else if (input.StartsWith("true"))
            {
                value = new JsonLiteral(true);
                return input.Buffer[4..];
            }
            else if (input.StartsWith("false"))
            {
                value = new JsonLiteral(false);
                return input.Buffer[5..];
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

                document = new (root: n);
                return true;
            }
            catch (ParserException e)
            {
                document = new (error: new ParserError(e, json));
                return false;
            }
        }
    }

    [TestClass]
    public class TestJson
    {
        readonly string[] GoodJson =
        {
            """
            {
            "type": "FeatureCollection",
            "features": [
                { "type": "Feature", "properties": { "MAPBLKLOT": "0001001", "BLKLOT": "0001001", "BLOCK_NUM": "0001", "LOT_NUM": "001", "FROM_ST": "0", "TO_ST": "0", "STREET": "UNKNOWN", "ST_TYPE": null, "ODD_EVEN": "E" }, "geometry": { "type": "Polygon", "coordinates": [ [ [ -122.422003528252475, 37.808480096967251, 0.0 ], [ -122.422076013325281, 37.808835019815085, 0.0 ], [ -122.421102174348633, 37.808803534992904, 0.0 ], [ -122.421062569067274, 37.808601056818148, 0.0 ], [ -122.422003528252475, 37.808480096967251, 0.0 ] ] ] } }
              ]
            }
            """,
            """
            {
                "PositiveNumber" : 512.52,
                "NegativeNumber": -100,
                "ExponentNumber": 1.2e-3,
                "Array": [ "Item1", "Item2" ],
                "Null": null,
                "Obj": { "Prop1": "Value" }
            }
            """
        };

        readonly string[] BadJson =
        {
            // Line 5, missing end ']'
            """
            {
                "PositiveNumber": 512.52,
                "NegativeNumber": -100,
                "Array": [ "Item1", "Item2",
                "Null": null,
                "Obj": { "Prop1": "Value" }
            }
            """
        };

        [TestMethod]
        public void TestToString()
        {
            var doc = new JsonObject([
                ("TestValue", new JsonLiteral(5)),
                ("TestArray", new JsonArray(new List<JsonValue>
                {
                    new JsonLiteral(1),
                    new JsonLiteral(2),
                    new JsonLiteral(3),
                    new JsonLiteral(4),
                })),
                ("NextValue", null)
            ]);

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