using ConsoleAppFramework;
using CreateIndex;

ConsoleApp.Run(args, CreateIndex);

/// <summary>
/// Creates a markdown TOC from the specified template and source files, respecting directory-based hierarchy.
/// </summary>
/// <param name="root">Relative path to the root of the repository. This will be prepended to all links in the generated TOC.</param>
/// <param name="src">Path to the source files directory. This directory will be scanned and parsed for markdown files to include in the TOC.</param>
/// <param name="clean">Set this flag if you only want to clean old generated code without emitting new code.</param>
static void CreateIndex(string root, string src, bool clean = false)
{
    ValidateArguments(root, src);
    IndexProcessor processor = new();
    processor.CreateIndex(root, src, clean);
}

static void ValidateArguments(string root, string src)
{
    ArgumentException.ThrowIfNullOrEmpty(root);
    ArgumentException.ThrowIfNullOrEmpty(src);
    if (!Directory.Exists(src))
    {
        throw new FileNotFoundException($"Source file not found: {src}");
    }
}
