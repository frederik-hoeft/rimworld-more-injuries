using MoreInjuries.WikiGen.Model;

namespace MoreInjuries.WikiGen.Nodes;

internal sealed class DirectoryNode(string name, string? displayName, int level, DirectoryInfo info) : Node(name, displayName, level)
{
    public INode? Descriptor { get; set; }

    public override FileSystemInfo FileSystemInfo => info;

    public override string? DisplayName => base.DisplayName ?? Descriptor?.DisplayName;

    public override void AddChild(FileInfo file, ReadOnlySpan<char> remainingPath)
    {
        remainingPath = GetNextSegment(remainingPath);
        ReadOnlySpan<char> nextName = GetNextName(remainingPath);
        if (TryGetDirectChild(nextName, out INode? child))
        {
            child.AddChild(file, remainingPath);
            return;
        }
        // need to create this child
        if (nextName.SequenceEqual(file.Name))
        {
            // check if this is an acual stand-alone entry, or a descriptor of this directory
            if (nextName is "README.md")
            {
                Descriptor = new FileNode(name: nextName.ToString(), displayName: null, Level, info: file)
                {
                    Parent = this
                };
            }
            else
            {
                Children.Add(new FileNode(name: nextName.ToString(), displayName: null, level: Level + 1, info: file)
                {
                    Parent = this
                });
            }
            return;
        }
        // recurse
        DirectoryNode newChild = new(name: nextName.ToString(), displayName: null, level: Level + 1, info: new DirectoryInfo(Path.Join(info.FullName, nextName)))
        {
            Parent = this
        };
        Children.Add(newChild);
        newChild.AddChild(file, remainingPath);
    }

    public override string GetLink(string? childSegment)
    {
        if (string.IsNullOrEmpty(childSegment))
        {
            return Descriptor?.GetLink() ?? $"/{Name}";
        }
        childSegment = $"/{Name}{childSegment}";
        if (Parent is not null)
        {
            return Parent.GetLink(childSegment);
        }
        return childSegment;
    }

    public override void Initialize()
    {
        Descriptor?.Initialize();
        Console.WriteLine($"Parsing node '{GetLink()}' (directory)");
        foreach (INode child in Children)
        {
            child.Initialize();
        }
    }

    private ReadOnlySpan<char> GetNextSegment(ReadOnlySpan<char> remainingPath)
    {
        // if we are the root, remove everything up until we match
        if (Level == 0)
        {
            int match = remainingPath.IndexOf(info.FullName, StringComparison.Ordinal);
            if (match is not 0)
            {
                throw new InvalidOperationException("root path doesn't match");
            }
            // +1 for directory seperator char
            return remainingPath[(info.FullName.Length + 1)..];
        }
        if (remainingPath.StartsWith(Name) && remainingPath.Length > Name.Length + 1 && remainingPath[Name.Length] == Path.DirectorySeparatorChar)
        {
            return remainingPath[(Name.Length + 1)..];
        }
        throw new InvalidOperationException("path segment mismatch");
    }

    private static ReadOnlySpan<char> GetNextName(ReadOnlySpan<char> remainingPath)
    {
        int directoryIndex = remainingPath.IndexOf(Path.DirectorySeparatorChar);
        if (directoryIndex != -1)
        {
            return remainingPath[..directoryIndex];
        }
        return remainingPath;
    }

    public override void Process(bool clean)
    {
        Descriptor?.Process(clean);
        foreach (INode child in Children)
        {
            child.Process(clean);
        }
    }

    public override IEnumerable<TocNode> EnumerateTocEntries(TocParams? parameters, int currentLevel = 0)
    {
        if (Descriptor is not null)
        {
            // currentLevel because the descriptor is a proxy for this directory
            foreach (TocNode node in Descriptor.EnumerateTocEntries(parameters, currentLevel))
            {
                yield return node;
            }
        }
        foreach (INode child in Children.OrderBy(static node => node.Name))
        {
            foreach (TocNode node in child.EnumerateTocEntries(parameters, currentLevel + 1))
            {
                yield return node;
            }
        }
    }
}
