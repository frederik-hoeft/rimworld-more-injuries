namespace CreateIndex.Nodes.Processing;

public interface ILineProcessor
{
    bool TryProcess(INode node, List<string> lines, ref int index, bool clean);
}
