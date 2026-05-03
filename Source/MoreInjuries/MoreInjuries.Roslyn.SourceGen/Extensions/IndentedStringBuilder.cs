using System.Text;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal sealed class IndentedStringBuilder(StringBuilder builder, int indentLevel = 0, int indentSize = 4)
{
    public StringBuilder Raw => builder;

    public int IndentSize => indentSize;

    public int IndentLevel => indentLevel;

    public string IndentString { get; } = new(' ', indentSize * indentLevel);

    public IndentedStringBuilder IncreaseIndent(int levels = 1) => new(builder, indentLevel + levels, indentSize);

    public IndentedStringBuilder DecreaseIndent(int levels = 1) => new(builder, Math.Max(0, indentLevel - levels), indentSize);

    public IndentedStringBuilder Append(string text)
    {
        if (builder.Length == 0 || builder[^1] == '\n')
        {
            builder.Append(IndentString);
        }
        builder.Append(text);
        return this;
    }

    public IndentedStringBuilder AppendLine(string line)
    {
        Append(line).Raw.AppendLine();
        return this;
    }

    public IndentedStringBuilder AppendBlock(string block)
    {
        ReadOnlySpan<char> blockSpan = block.AsSpan();
        int startIndex = 0;
        int lineIndex;
        while (startIndex < blockSpan.Length && (lineIndex = blockSpan[startIndex..].IndexOf('\n')) != -1)
        {
            int endIndex = startIndex + lineIndex;
            // string possibly has \r\n line endings, so trim any trailing \r from the line
            builder.Append(IndentString).AppendLine(blockSpan[startIndex..endIndex].TrimEnd('\r').ToString());
            startIndex = endIndex + 1;
        }
        if (startIndex < blockSpan.Length)
        {
            builder.Append(IndentString).AppendLine(blockSpan[startIndex..].TrimEnd('\r').ToString());
        }
        return this;
    }
}
