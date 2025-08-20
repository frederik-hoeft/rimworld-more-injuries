namespace MoreInjuries.WikiGen.Nodes.Processing;

internal interface ILineProcessor
{
    bool TryProcess(INode node, List<string> lines, ref int index, bool clean);
}
