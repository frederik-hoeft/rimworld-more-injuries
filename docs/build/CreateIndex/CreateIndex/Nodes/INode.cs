using CreateIndex.Model;

namespace CreateIndex.Nodes;

internal interface INode
{
    string Name { get; }

    string? DisplayName { get; }

    int Level { get; }

    INode? Parent { get; }

    IReadOnlyList<INode> Children { get; }

    FileSystemInfo FileSystemInfo { get; }

    string GetLink();

    string GetLink(string? childSegment);

    void AddChild(FileInfo file, ReadOnlySpan<char> remainingPath);

    void Initialize();

    void Process(bool clean);

    INode? Resolve(ReadOnlySpan<char> path);

    INode GetRoot();

    IEnumerable<TocNode> EnumerateTocEntries(TocParams? parameters, int currentLevel = 0);
}