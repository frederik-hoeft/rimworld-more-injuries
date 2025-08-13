using CreateIndex.Nodes;

namespace CreateIndex;

public class IndexProcessor
{
    /// <summary>
    /// Creates a markdown TOC from the specified template and source files, respecting directory-based hierarchy.
    /// </summary>
    /// <param name="root">Relative path to the root of the repository. This will be prepended to all links in the generated TOC.</param>
    /// <param name="src">Path to the source files directory. This directory will be scanned and parsed for markdown files to include in the TOC.</param>
    /// <param name="clean">If <see langword="true"/> only cleans generated code and exits.</param>
    public void CreateIndex(string root, string src, bool clean)
    {
        DirectoryInfo source = new(src);
        FileInfo[] files = source.GetFiles("*.md", SearchOption.AllDirectories);
        DirectoryNode rootNode = new(name: root, displayName: null, level: 0, info: source);
        foreach (FileInfo file in files)
        {
            rootNode.AddChild(file, file.FullName);
        }
        rootNode.Initialize();
        rootNode.Process(clean);
    }
}
