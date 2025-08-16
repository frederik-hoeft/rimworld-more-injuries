using MoreInjuries.WikiGen.Model;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MoreInjuries.WikiGen.Nodes;

internal abstract class Node(string name, string? displayName, int level) : INode
{
    protected List<INode> Children { get; } = [];

    public string Name => name;

    public virtual string? DisplayName => displayName;

    public int Level => level;

    public INode? Parent { get; set; }

    IReadOnlyList<INode> INode.Children => Children;

    public abstract FileSystemInfo FileSystemInfo { get; }

    public virtual string GetLink() => GetLink(childSegment: null);

    public abstract string GetLink(string? childSegment);

    public abstract void AddChild(FileInfo file, ReadOnlySpan<char> remainingPath);

    public abstract void Initialize();

    public abstract void Process(bool clean);

    protected bool TryGetDirectChild(ReadOnlySpan<char> name, [NotNullWhen(true)] out INode? child)
    {
        foreach (INode node in Children)
        {
            if (name.SequenceEqual(node.Name))
            {
                child = node;
                return true;
            }
        }
        child = null;
        return false;
    }

    public virtual INode? Resolve(ReadOnlySpan<char> path)
    {
        if (path is "." or "./")
        {
            return GetCurrentDirectory();
        }
        if (path is "$self")
        {
            return this;
        }
        if (path.StartsWith("/"))
        {
            // resolve absolute paths from the root
            return GetRoot().Resolve(path[1..]);
        }
        if (path.StartsWith("./"))
        {
            // normalize relative paths
            path = path[2..];
        }
        if (path.Length == 0)
        {
            return this;
        }
        path = ConsumeSegment(path, out ReadOnlySpan<char> segment);
        foreach (INode node in Children)
        {
            if (segment.SequenceEqual(node.Name))
            {
                return node.Resolve(path);
            }
        }
        // not found :C
        return null;
    }

    protected static ReadOnlySpan<char> ConsumeSegment(ReadOnlySpan<char> path, out ReadOnlySpan<char> segment)
    {
        if (path.Length == 0)
        {
            Debug.Fail("attempting to consume empty path segment");
            segment = [];
            return path;
        }
        int index = path.IndexOf('/');
        if (index == -1)
        {
            segment = path;
            return [];
        }
        segment = path[..index];
        return path[(segment.Length + 1)..];
    }

    public virtual INode GetRoot()
    {
        INode node;
        for (node = this; node.Parent is not null; node = node.Parent) { }
        Debug.Assert(node.Level == 0);
        return node;
    }

    public abstract IEnumerable<TocNode> EnumerateTocEntries(TocParams? parameters, int currentLevel = 0);

    protected DirectoryNode GetCurrentDirectory()
    {
        for (INode? node = this; node is not null; node = node.Parent)
        {
            if (node is DirectoryNode directoryNode)
            {
                return directoryNode;
            }
        }
        throw new InvalidOperationException($"unable to determine current directory for node '{GetLink()}'");
    }
}