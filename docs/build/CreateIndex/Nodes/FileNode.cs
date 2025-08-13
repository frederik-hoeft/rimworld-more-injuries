using CreateIndex.Model;
using CreateIndex.Nodes.Processing;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CreateIndex.Nodes;

public sealed partial class FileNode(string name, string? displayName, int level, FileInfo info) : Node(name, displayName, level)
{
    private const string CODE_BLOCK_BOUNDARY = "```";
    private const string COMMENT_START = "<!--";
    private const string COMMENT_END = "-->";

    private static readonly ImmutableArray<ILineProcessor> s_processors =
    [
        new TocLineProcessor(),
        new BreadcrumbProcessor(),
        new ToTopProcessor(),
    ];

    private readonly List<string> _lines = 
    [
        .. File.ReadAllLines(info.FullName)
    ];

    [GeneratedRegex(@"^\s*(?<level>#{1,6})\s+(?<name>.*?)$")]
    private partial Regex MarkdownHeaderRegex { get; }

    public override FileSystemInfo FileSystemInfo => info;

    public override string? DisplayName
    {
        get
        {
            if (!string.IsNullOrEmpty(base.DisplayName))
            {
                return base.DisplayName;
            }
            if (Children is [INode single])
            {
                return single.DisplayName;
            }
            return null;
        }
    }

    public override void AddChild(FileInfo file, ReadOnlySpan<char> remainingPath) => throw new NotSupportedException();

    public override void Initialize()
    {
        Console.WriteLine($"Parsing node '{GetLink()}'");
        HashSet<string> namesInUse = [];
        Stack<MarkdownNode> nodes = new(capacity: 6);
        IMarkdownParserState parserState = new NormalState();
        foreach (string line in _lines)
        {
            ReadOnlySpan<char> lineSpan = line;
            parserState = parserState.ConsumeLine(ref lineSpan, out bool validTitleLine);
            // line must have been consumed entirely
            Debug.Assert(lineSpan.Length == 0);
            if (validTitleLine && MarkdownHeaderRegex.Match(line) is { Success: true } match)
            {
                Group level = match.Groups["level"];
                Group name = match.Groups["name"];
                if (level.ValueSpan.Length > nodes.Count + 1)
                {
                    throw new InvalidOperationException($"Hierarchy violation while processing section '{name.ValueSpan}' in file {info.Name}");
                }
                while (nodes.Count >= level.ValueSpan.Length)
                {
                    nodes.Pop();
                }
                MarkdownNode newNode = CreateChild(namesInUse, name.Value.Trim(), level.ValueSpan.Length);
                if (nodes.TryPeek(out MarkdownNode? top))
                {
                    top.AddChild(newNode);
                }
                else
                {
                    Children.Add(newNode);
                    newNode.Parent = this;
                }
                nodes.Push(newNode);
                newNode.Initialize();
            }
        }
    }

    private MarkdownNode CreateChild(HashSet<string> namesInUse, string displayName, int markdownHeaderLevel)
    {
        string normalizedRaw = Normalize(displayName);
        string normalized = normalizedRaw;
        for (int i = 1; !namesInUse.Add(normalized); ++i)
        {
            normalized = $"{normalizedRaw}-{i}";
        }
        return new MarkdownNode(normalized, displayName, Level +  markdownHeaderLevel, info);
    }

    private static string Normalize(string markdownDisplayName)
    {
        StringBuilder stringBuilder = new();
        char previous = '\0';
        foreach (char c in markdownDisplayName)
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                if (char.IsAsciiLetterUpper(c))
                {
                    stringBuilder.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
            else if (c is ' ' && previous is not ' ')
            {
                stringBuilder.Append('-');
            }
            else if (c is '-' && stringBuilder.Length > 0 && stringBuilder[^1] != '-')
            {
                stringBuilder.Append('-');
            }
            previous = c;
        }
        return stringBuilder.ToString();
    }

    public override string GetLink(string? childSegment)
    {
        if (!string.IsNullOrEmpty(childSegment))
        {
            childSegment = $"#{childSegment}";
        }
        childSegment = $"/{Name}{childSegment}";
        // files always have a parent
        Debug.Assert(Parent is not null);
        return Parent.GetLink(childSegment);
    }

    public override void Process(bool clean)
    {
        Console.WriteLine($"Processing {GetLink()}...");
        for (int i = 0; i < _lines.Count; ++i)
        {
            foreach (ILineProcessor processor in s_processors)
            {
                if (processor.TryProcess(this, _lines, ref i, clean))
                {
                    break;
                }
            }
        }
        File.WriteAllLines(info.FullName, _lines);
    }

    public override IEnumerable<TocNode> EnumerateTocEntries(TocParams? parameters, int currentLevel = 0)
    {
        foreach (INode child in Children)
        {
            // currentLevel because files themselves can't be listed in the toc, only the titles of a file can
            foreach (TocNode node in child.EnumerateTocEntries(parameters, currentLevel))
            {
                yield return node;
            }
        }
    }

    private interface IMarkdownParserState
    {
        IMarkdownParserState ConsumeLine(ref ReadOnlySpan<char> line, out bool validTitleLine);
    }

    private readonly struct NormalState : IMarkdownParserState
    {
        public IMarkdownParserState ConsumeLine(ref ReadOnlySpan<char> line, out bool validTitleLine)
        {
            for (; line.Length > 0; line = line[1..])
            {
                if (line.StartsWith(CODE_BLOCK_BOUNDARY))
                {
                    line = line[CODE_BLOCK_BOUNDARY.Length..];
                    CodeBlockState codeBlock = new();
                    return codeBlock.ConsumeLine(ref line, out validTitleLine);
                }
                if (line.StartsWith(COMMENT_START))
                {
                    line = line[COMMENT_START.Length..];
                    CommentState comment = new();
                    return comment.ConsumeLine(ref line, out validTitleLine);
                }
            }
            validTitleLine = true;
            return this;
        }
    }

    private readonly struct CodeBlockState : IMarkdownParserState
    {
        public IMarkdownParserState ConsumeLine(ref ReadOnlySpan<char> line, out bool validTitleLine)
        {
            // a code block is never a valid title line
            validTitleLine = false;
            for (; line.Length > 0; line = line[1..])
            {
                if (line.StartsWith(CODE_BLOCK_BOUNDARY))
                {
                    line = line[CODE_BLOCK_BOUNDARY.Length..];
                    NormalState normalState = new();
                    return normalState.ConsumeLine(ref line, out _);
                }
            }
            // we're still in the code block
            return this;
        }
    }

    private readonly struct CommentState : IMarkdownParserState
    {
        public IMarkdownParserState ConsumeLine(ref ReadOnlySpan<char> line, out bool validTitleLine)
        {
            for (; line.Length > 0; line = line[1..])
            {
                if (line.StartsWith(COMMENT_END))
                {
                    line = line[COMMENT_END.Length..];
                    NormalState normalState = new();
                    return normalState.ConsumeLine(ref line, out validTitleLine);
                }
            }
            // a comment line is never a valid header
            validTitleLine = false;
            return this;
        }
    }
}
