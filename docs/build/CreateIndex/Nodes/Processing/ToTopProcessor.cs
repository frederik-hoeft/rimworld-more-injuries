using CreateIndex.Model;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CreateIndex.Nodes.Processing;

public sealed partial class ToTopProcessor : LineProcessorBase
{
    [GeneratedRegex(@"^<!--\s+@generate_link_to_top\s+(?<params>.*)\s*-->$")]
    protected override partial Regex GeneratedBlockStartRegex { get; }

    protected override void Process(INode node, Match match, List<string> lines, ref int index, bool clean)
    {
        Console.WriteLine($"Emitting link to top in '{node.GetLink()}'");
        ToTopParams? parameters = null;
        if (match.Groups.TryGetValue("params", out Group? args) && args.ValueSpan.Trim() is { IsEmpty: false } json)
        {
            parameters = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.ToTopParams);
        }
        // we know that this node is a FileNode, since only files can be processed.
        if (node is not (FileNode and INode { Children: [INode firstAnchor, ..] }))
        {
            throw new InvalidOperationException("cannot generate link to top since no valid anchor exists to link to");
        }
        string? title = firstAnchor.DisplayName ?? "back to the top";
        string link = $"#{firstAnchor.Name}";
        string markdown;
        if (!string.IsNullOrEmpty(parameters?.Template))
        {
            markdown = string.Format(parameters.Template, title, link);
        }
        else
        {
            markdown = $"[back to the top]({link})";
        }
        Emit(node, lines, ref index, [markdown], clean);
    }
}