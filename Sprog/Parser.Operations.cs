using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Wivuu.Sprog;

public partial struct Parser
{
    #region MatchWhile

    /// <summary>
    /// Iterate through input until the predicate returns false
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <returns>Index match end</returns>
    int MatchWhile(Predicate predicate)
    {
        var buffer = Buffer;
        var len    = buffer.Length;

        var i = 0;
        while (i < len && predicate(buffer[i]))
            ++i;

        return i;
    }

    /// <summary>
    /// Iterate through input until the predicate returns false or number of taken
    /// characters taken is met
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <param name="take">Number of characters to take</param>
    /// <returns>Index match end</returns>
    int MatchWhile(Predicate predicate, int take)
    {
        var buffer = Buffer;
        var len    = Math.Min(buffer.Length, take);

        var i = 0;
        while (i < len && predicate(buffer[i]))
            ++i;

        return i;
    }

    #endregion

    #region Take

    /// <summary>
    /// Take one character
    /// </summary>
    /// <param name="match">Matching character</param>
    /// <returns>Remainder of input</returns>
    public Parser Take(out char match)
    {
        var buffer = Buffer;
        if (buffer.Length > 0)
        {
            match = buffer[0];
            return buffer[1..];
        }
        else
        {
            match = default;
            return buffer;
        }
    }

    /// <summary>
    /// Take one character, if matching
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <param name="match">Matching character</param>
    /// <returns>Remainder of input</returns>
    public Parser Take(Predicate predicate, out char match)
    {
        var buffer = Buffer;
        if (buffer.Length > 0 && predicate(buffer[0]))
        {
            match = buffer[0];
            return buffer[1..];
        }
        else
        {
            match = default;
            return buffer;
        }
    }

    /// <summary>
    /// Take multiple characters, while matching
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <param name="match">Matching characters</param>
    /// <returns>Remainder of input</returns>
    public Parser Take(Predicate predicate, out ReadOnlySpan<char> match)
    {
        var i = MatchWhile(predicate);
        match = Buffer[..i];

        return Buffer[i..];
    }

    /// <summary>
    /// Take multiple characters, while matching
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <param name="match">Matching characters</param>
    /// <returns>Remainder of input</returns>
    public Parser Take(Predicate predicate, out string match)
    {
        var i = MatchWhile(predicate);
        match = Buffer[..i].ToString();

        return Buffer[i..];
    }

    #endregion

    #region TakeWhile

    /// <summary>
    /// Take multiple items, while matching
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <param name="match">Matching items</param>
    /// <returns>Remainder of input</returns>
    public Parser TakeMany<T>(TakeManyCondition<T> predicate, out List<T> match)
    {
        var input = this;
        match     = [];

        do
        {
            var (success, value) = predicate(ref input);

            if (success)
                match.Add(value);
            else
                break;
        }
        while (true);

        return input;
    }

    #endregion

    #region Peek

    /// <summary>
    /// Peek multiple characters
    /// </summary>
    /// <param name="length">Input test</param>
    /// <param name="match">Matching span</param>
    /// <returns>True if enough characters to match; otherwise false</returns>
    public Parser Peek(int length, out ReadOnlySpan<char> match)
    {
        match = Buffer.Length < length
            ? default
            : Buffer[..length];

        return this;
    }

    /// <summary>
    /// Peek multiple characters
    /// </summary>
    /// <param name="length">Input test</param>
    /// <param name="match">Matching string</param>
    /// <returns>True if enough characters to match; otherwise false</returns>
    public Parser Peek(int length, out string? match)
    {
        match = Buffer.Length < length
            ? default
            : Buffer[..length].ToString();

        return this;
    }

    /// <summary>
    /// Peek single character
    /// </summary>
    /// <param name="match">Matching character</param>
    /// <returns>True if enough characters to match; otherwise false</returns>
    public Parser Peek(out char match)
    {
        match = Buffer.Length == 0
            ? default
            : Buffer[0];

        return this;
    }

    #endregion

    #region Skip

    /// <summary>
    /// Skip one character
    /// </summary>
    /// <returns>Remainder of input</returns>
    public Parser SkipOne() =>
        Buffer.Length > 0
        ? Buffer[1..]
        : this;

    /// <summary>
    /// Skip one character if predicate is true
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <returns>Remainder of input</returns>
    public Parser SkipOne(char predicate) =>
        Buffer.Length > 0 && Buffer[0] == predicate
        ? Buffer[1..]
        : this;

    /// <summary>
    /// Skip one character if predicate is true
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <returns>Remainder of input</returns>
    public Parser SkipOne(Predicate predicate) =>
        Buffer.Length > 0 && predicate(Buffer[0])
        ? Buffer[1..]
        : this;

    /// <summary>
    /// Test if the input starts with the input value before skipping
    /// </summary>
    /// <param name="value">Input pattern</param>
    /// <param name="skipped">Input was skipped</param>
    /// <returns>True if the input matches the pattern</returns>
    public Parser Skip(ReadOnlySpan<char> value, out bool skipped) =>
        (skipped = StartsWith(value))
        ? Skip(value)
        : this;

    /// <summary>
    /// Test if the input starts with the input value before skipping
    /// </summary>
    /// <param name="value">Input pattern</param>
    /// <param name="skipped">Input was skipped</param>
    /// <>True if the input matches the pattern</returns>
    public Parser Skip(char value, out bool skipped) =>
        (skipped = StartsWith(value))
        ? SkipOne()
        : this;

    /// <summary>
    /// Skip while predicate is true
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <returns>Remainder of input</returns>
    public Parser Skip(Predicate predicate) =>
        Buffer[MatchWhile(predicate)..];

    /// <summary>
    /// Skip while predicate is true
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <returns>Remainder of input</returns>
    public Parser Skip(ReadOnlySpan<char> predicate)
    {
        var buffer = Buffer;
        if (buffer.Length < predicate.Length)
            return buffer;

        int i;
        for (i = 0; i < predicate.Length; ++i)
        {
            if (buffer[i] != predicate[i])
                return this;
        }

        return buffer[i..];
    }

    /// <summary>
    /// Skip until predicate is matched
    /// </summary>
    /// <param name="predicate">Input test</param>
    /// <returns>Remainder of input</returns>
    public Parser SkipUntil(char predicate)
    {
        bool neq(char c) => c != predicate;

        return Skip(neq);
    }

    /// <summary>
    /// Skip if the predicate is matched, returning true if skipped
    /// </summary>
    /// <param name="value">Input pattern</param>
    /// <param name="rest">Remaining buffer</param>
    /// <returns>True if the input matches the pattern</returns>
    public bool SkipIf(Predicate predicate, out Parser rest)
    {
        var buffer = Buffer;
        if (buffer.Length > 0 && predicate(buffer[0]))
        {
            rest = buffer[1..];
            return true;
        }
        else
        {
            rest = buffer;
            return false;
        }
    }

    /// <summary>
    /// Skip if the predicate is matched, returning true if skipped
    /// </summary>
    /// <param name="predicate">Input pattern</param>
    /// <param name="rest">Remaining buffer</param>
    /// <returns>True if the input matches the pattern</returns>
    public bool SkipIf(ReadOnlySpan<char> predicate, out Parser rest)
    {
        var buffer = Buffer;
        if (buffer.StartsWith(predicate))
        {
            rest = buffer[predicate.Length..];
            return true;
        }
        else
        {
            rest = buffer;
            return false;
        }
    }

    #endregion

    #region Let

    /// <summary>
    /// Assign and return remaining buffer
    /// </summary>
    /// <param name="id">Assignments</param>
    /// <returns>Remaining buffer</returns>
    public Parser Let(ReadOnlySpan<char> id) =>
        this;

    /// <summary>
    /// Assign and return remaining buffer
    /// </summary>
    /// <param name="id">Assignments</param>
    /// <returns>Remaining buffer</returns>
    public Parser Let(string id) =>
        this;

    /// <summary>
    /// Assign and return remaining buffer
    /// </summary>
    /// <param name="id">Assignments</param>
    /// <returns>Remaining buffer</returns>
    public Parser Let(Parser id) =>
        id;

    /// <summary>
    /// Assign and return remaining buffer
    /// </summary>
    /// <param name="id">Assignments</param>
    /// <returns>Remaining buffer</returns>
    public Parser Let<T>(T id) =>
        this;

    #endregion

    #region Declare

    /// <summary>
    /// Assign and return remaining buffer
    /// </summary>
    /// <param name="output">Newly declared output</param>
    /// <param name="input">Input</param>
    /// <returns>Remaining buffer</returns>
    public Parser Declare(out string output, string input)
    {
        output = input;
        return this;
    }

    /// <summary>
    /// Assign and return remaining buffer
    /// </summary>
    /// <param name="output">Newly declared output</param>
    /// <param name="input">Input</param>
    /// <returns>Remaining buffer</returns>
    public Parser Declare(out char output, char input)
    {
        output = input;
        return this;
    }

    /// <summary>
    /// Assign and return remaining buffer
    /// </summary>
    /// <param name="output">Newly declared output</param>
    /// <param name="input">Input</param>
    /// <returns>Remaining buffer</returns>
    public Parser Declare<T>(out T output, T input)
    {
        output = input;
        return this;
    }

    #endregion

    #region Rest

    /// <summary>
    /// Return remaining buffer as 'out'
    /// </summary>
    /// <param name="rest">Remaining buffer</param>
    /// <returns>Remaining buffer</returns>
    public Parser Rest(out Parser rest) =>
        rest = this;

    #endregion

    #region If

    /// <summary>
    /// Return true if condition is true, or false if condition is false
    /// </summary>
    /// <returns>True or false</returns>
    public bool If(bool condition) =>
        condition;

    /// <summary>
    /// Return true if condition is true, or false if condition is false
    /// </summary>
    /// <param name="rest">Remaining buffer</param>
    /// <returns>True or false</returns>
    public bool If(bool condition, out Parser rest)
    {
        rest = this;
        return condition;
    }

    #endregion

    #region Assert

    /// <summary>
    /// Create an assertion
    /// </summary>
    /// <returns>Parser</returns>
    public Parser Assert(ReadOnlySpan<char> assertion)
    {
        if (assertion.Length > 0)
            throw new ParserException(assertion.ToString(), Buffer.Length);

        return this;
    }

    /// <summary>
    /// Create an assertion
    /// </summary>
    /// <returns>Parser</returns>
    public Parser Assert([DoesNotReturnIf(false)] bool assertion, string failIfFalse)
    {
        if (!assertion)
            throw new ParserException(failIfFalse, Buffer.Length);

        return this;
    }

    /// <summary>
    /// Assertion fails
    /// </summary>
    public ParserException Fail(string assertion) =>
        new(assertion, Buffer.Length);

    #endregion

    #region Tests

    /// <summary>
    /// Check if parser has reached EOF
    /// </summary>
    /// <returns>Input buffer is 0</returns>
    public bool IsEOF() =>
        Buffer.Length == 0;

    /// <summary>
    /// Test if the input starts with the input value
    /// </summary>
    /// <param name="value">Input pattern</param>
    /// <returns>True if the input matches the pattern</returns>
    public bool StartsWith(ReadOnlySpan<char> value) => Buffer.StartsWith(value);

    /// <summary>
    /// Test if the input starts with the input value
    /// </summary>
    /// <param name="value">Input pattern</param>
    /// <returns>True if the input matches the pattern</returns>
    public bool StartsWith(char value) =>
        Buffer.Length > 0 && Buffer[0] == value;

    /// <summary>
    /// Test if the input starts with the input value
    /// </summary>
    /// <param name="predicate">Input callback</param>
    /// <returns>True if the input matches the pattern</returns>
    public bool StartsWith(Predicate predicate) =>
        Buffer.Length > 0 && predicate(Buffer[0]);

    #endregion

    #region Returns

    /// <summary>
    /// Return input value
    /// </summary>
    public T Return<T>(T value) => value;

    #endregion
}