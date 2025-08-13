using CreateIndex.Model;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CreateIndex.Nodes.Processing;

internal sealed partial class TocLineProcessor : LineProcessorBase
{
    [GeneratedRegex(@"^<!--\s+@generate_toc\s+(?<params>.*)\s*-->$")]
    protected override partial Regex GeneratedBlockStartRegex { get; }

    protected override void Process(INode node, Match match, List<string> lines, ref int index, bool clean)
    {
        TocParams? parameters = null;
        if (match.Groups.TryGetValue("params", out Group? args) && args.ValueSpan.Trim() is { IsEmpty: false } json)
        {
            parameters = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.TocParams);
        }
        ImmutableArray<string> generatedLines = [.. EmitToc(node, parameters)];
        Emit(node, lines, ref index, generatedLines, clean);
    }

    private static IEnumerable<string> EmitToc(INode node, TocParams? parameters)
    {
        parameters ??= new TocParams();
        Console.WriteLine($"Emitting TOC for '{parameters.Source}' with indent size {parameters.IndentSize} in {node.GetLink()}");
        INode tocRoot = node.Resolve(parameters.Source) ?? throw new InvalidOperationException($"unable to resolve invalid path '{parameters.Source}'");
        Console.WriteLine($"TOC root resolved to '{tocRoot.GetLink()}'");
        IndentCache indentCache = new(parameters.IndentSize);
        foreach ((INode entry, int indentLevel) in tocRoot.EnumerateTocEntries(parameters))
        {
            string indent = indentCache.GetIndent(indentLevel);
            string title = entry.DisplayName ?? "ERR: MISSING TEXT";
            string link = entry.GetLink();
            yield return $"{indent}- [{title}]({link})";
        }
    }
}