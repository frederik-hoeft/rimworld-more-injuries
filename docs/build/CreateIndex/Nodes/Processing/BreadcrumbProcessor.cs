using CreateIndex.Model;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CreateIndex.Nodes.Processing;

public sealed partial class BreadcrumbProcessor : LineProcessorBase
{
    [GeneratedRegex(@"^<!--\s+@generate_breadcrumb_trail\s+(?<params>.*)\s*-->$")]
    protected override partial Regex GeneratedBlockStartRegex { get; }

    protected override void Process(INode node, Match match, List<string> lines, ref int index, bool clean)
    {
        Console.WriteLine($"Emitting breadcrumbs for '{node.GetLink()}'");
        BreadcrumbParams? parameters = null;
        if (match.Groups.TryGetValue("params", out Group? args) && args.ValueSpan.Trim() is { IsEmpty: false } json)
        {
            parameters = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.BreadcrumbParams);
        }
        INode? root = null;
        if (parameters is { Root.Length: > 0 })
        {
            root = node.Resolve(parameters.Root);
        }
        root ??= node.GetRoot();
        Stack<INode> navStack = [];
        for (INode? current = node; current is not null; current = current.Parent)
        {
            navStack.Push(current);
            if (current == root || root is DirectoryNode { Descriptor: { } descriptor } && current == descriptor)
            {
                break;
            }
        }
        StringBuilder sb = new();
        string connector = parameters?.Connector ?? " > ";
        while (navStack.TryPop(out INode? current))
        {
            if (!string.IsNullOrEmpty(current.DisplayName))
            {
                if (sb.Length > 0)
                {
                    sb.Append(connector);
                }
                sb.Append($"[{current.DisplayName}]({current.GetLink()})");
            }
        }
        string breadcrumbTrail = sb.ToString();
        if (!string.IsNullOrEmpty(parameters?.Template))
        {
            breadcrumbTrail = string.Format(parameters.Template, breadcrumbTrail);
        }
        Emit(node, lines, ref index, [breadcrumbTrail], clean);
    }
}