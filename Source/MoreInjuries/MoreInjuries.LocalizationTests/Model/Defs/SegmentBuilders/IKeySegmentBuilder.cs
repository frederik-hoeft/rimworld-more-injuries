using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Model.Defs.SegmentBuilders;

internal interface IKeySegmentBuilder
{
    bool TryHandleSegment(ref XElement segment, XElement defNode, List<string> path);
}
