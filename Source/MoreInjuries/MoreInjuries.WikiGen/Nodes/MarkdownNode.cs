using MoreInjuries.WikiGen.Model;
using System.Diagnostics;

namespace MoreInjuries.WikiGen.Nodes;

internal sealed class MarkdownNode(string name, string? displayName, int level, FileInfo info) : Node(name, displayName, level)
{
    public override FileSystemInfo FileSystemInfo => info;

    public override void AddChild(FileInfo file, ReadOnlySpan<char> remainingPath) => throw new NotSupportedException();

    public void AddChild(MarkdownNode node)
    {
        Children.Add(node);
        node.Parent = this;
    }

    public override string GetLink(string? childSegment)
    {
        // markdown nodes are always at least in a file lol
        Debug.Assert(Parent is not null);
        if (string.IsNullOrEmpty(childSegment))
        {
            childSegment = Name;
        }
        return Parent.GetLink(childSegment);
    }

    public override void Initialize() => Console.WriteLine($"Parsing node '{GetLink()}'");

    public override IEnumerable<TocNode> EnumerateTocEntries(TocParams? parameters, int currentLevel = 0)
    {
        yield return new TocNode(this, currentLevel);
        foreach (INode child in Children)
        {
            foreach (TocNode node in child.EnumerateTocEntries(parameters, currentLevel + 1))
            {
                yield return node;
            }
        }
    }

    public override void Process(bool clean) { }
}