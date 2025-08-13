using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace CreateIndex.Nodes.Processing;

public abstract partial class LineProcessorBase : ILineProcessor
{
    protected abstract Regex GeneratedBlockStartRegex { get; }

    [GeneratedRegex(@"^<!--\s+@end_generated_block\s+-->$")]
    protected partial Regex GeneratedBlockEndRegex { get; }

    protected abstract void Process(INode node, Match match, List<string> lines, ref int index, bool clean);

    protected virtual void Emit(INode node, List<string> lines, ref int lineIndex, ImmutableArray<string> generatedLines, bool clean)
    {
        for (int startIndex = lineIndex; lineIndex < lines.Count; ++lineIndex)
        {
            string line = lines[lineIndex];
            // we're scanning for the end of the generated code...
            if (GeneratedBlockEndRegex.IsMatch(line))
            {
                // found the end, remove everything in-between
                lines.RemoveRange(startIndex + 1, lineIndex - (startIndex + 1));
                if (clean)
                {
                    lineIndex = startIndex;
                }
                else
                {
                    lines.InsertRange(startIndex + 1, generatedLines);
                    // reset iterator and continue
                    lineIndex = startIndex + generatedLines.Length;
                }
                return;
            }
        }
        throw new InvalidOperationException($"Parser failure in {node.GetLink()}: EOF while executing {GetType().Name}");
    }

    public virtual bool TryProcess(INode node, List<string> lines, ref int index, bool clean)
    {
        string line = lines[index];
        if (GeneratedBlockStartRegex.Match(line) is not { Success: true } match)
        {
            return false;
        }
        Console.WriteLine($"Line processor match for {GetType().Name} in {node.GetLink()}");
        Process(node, match, lines, ref index, clean);
        return true;
    }
}
