using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Model.Defs.SegmentBuilders;

internal sealed class DefaultSegmentBuilder : IKeySegmentBuilder
{
    public bool TryHandleSegment(ref XElement segment, XElement defNode, List<string> path)
    {
        path.Add(segment.Name.LocalName);
        return true;
    }
}
