﻿
namespace LemonKit.Generators.Writers;

internal sealed class CodeWriter : IDisposable
{

    private const string DefaultIndent = "\t";
    private const string DefaultNewLine = "\n";

    public string Indent { get; set; } = DefaultIndent;
    public string NewLine { get; set; } = DefaultNewLine;

    private string[] _LevelCache;

    private ArrayBuilder<char> Builder { get; set; } = new ArrayBuilder<char>();
    private int CurrentLevel { get; set; } = 0;
    private string CurrentLevelString { get; set; } = "";

    public CodeWriter()
    {

        _LevelCache = new string[6];
        _LevelCache[0] = "";

        for(int e = 1; e < _LevelCache.Length; e++)
        {
            _LevelCache[e] = _LevelCache[e - 1] + Indent;
        }

    }

    public void UpIndent()
    {

        CurrentLevel++;
        if(CurrentLevel == _LevelCache.Length)
        {
            Array.Resize(ref _LevelCache, _LevelCache.Length * 2);
        }

        CurrentLevelString = _LevelCache[CurrentLevel]
            ??= _LevelCache[CurrentLevel - 1] + Indent;

    }

    public void DownIndent()
    {

        CurrentLevel--;
        CurrentLevelString = _LevelCache[CurrentLevel];

    }

    public Span<char> Advance(int size)
    {

        AddIndentOnDemand();
        return Builder.Advance(size);

    }

    public void WriteText(string text)
        => WriteText(text.AsSpan());

    public void WriteText(ReadOnlySpan<char> text)
    {

        AddIndentOnDemand();
        Builder.AddRange(text);

    }

    public void Write(string text, bool multiLine = false)
        => Write(text.AsSpan(), multiLine);

    public void Write(ReadOnlySpan<char> content, bool multiLine = false)
    {

        if(!multiLine)
        {

            WriteText(content);

        }
        else
        {

            while(content.Length > 0)
            {

                int newLinePosition = content.IndexOf(NewLine[0]);

                if(newLinePosition >= 0)
                {

                    ReadOnlySpan<char> line = content[..newLinePosition];

                    WriteIf(!line.IsEmpty, line);
                    WriteLine();

                    content = content[(newLinePosition + 1)..];

                }
                else
                {

                    WriteText(content);
                    break;

                }

            }

        }

    }

    public void WriteIf(bool condition, string content, bool multiLine = false)
        => WriteIf(condition, content.AsSpan(), multiLine);

    public void WriteIf(bool condition, ReadOnlySpan<char> content, bool multiLine = false)
    {

        if(condition)
        {
            Write(content, multiLine);
        }

    }

    public void WriteLine(string content, bool multiLine = false)
        => WriteLine(content.AsSpan(), multiLine);

    public void WriteLine(ReadOnlySpan<char> content, bool multiLine = false)
    {

        Write(content, multiLine);
        WriteLine();

    }

    public void WriteLineIf(bool condition, string content, bool multiLine = false)
        => WriteLineIf(condition, content.AsSpan(), multiLine);

    public void WriteLineIf(bool condition, ReadOnlySpan<char> content, bool multiLine = false)
    {

        if(condition)
        {
            WriteLine(content, multiLine);
        }

    }

    public void WriteLine()
    {

        Builder.Add(NewLine[0]);

    }

    public override string ToString()
    {

        return Builder.Span.Trim().ToString();

    }

    public void Dispose()
    {

        Builder.Dispose();

    }

    private void AddIndentOnDemand()
    {

        if(Builder.Count == 0 || Builder.Span[^1] == NewLine[0])
        {
            Builder.AddRange(CurrentLevelString.AsSpan());
        }

    }

}

