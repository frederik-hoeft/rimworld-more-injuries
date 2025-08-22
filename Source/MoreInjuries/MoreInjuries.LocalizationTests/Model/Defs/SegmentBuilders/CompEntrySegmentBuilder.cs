using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Model.Defs.SegmentBuilders;

internal sealed class CompEntrySegmentBuilder : IKeySegmentBuilder
{
    public bool TryHandleSegment(ref XElement segment, XElement defNode, List<string> path)
    {
        if (defNode is { Name.LocalName: "HediffDef" or "ThingDef" }
            && segment is { Name.LocalName: "li", Parent.Name.LocalName: "comps", HasAttributes: true }
            && segment.Attribute("Class") is { Value: string compProps })
        {
            // instead of [...].comps.li.[...], we want [...].comps.<compClass>.[...]
            string compClass = GetCompClass(compProps);
            path.Add(compClass);
            return true;
        }
        return false;
    }

    private static string GetCompClass(string compProps) => compProps.Replace("Properties", string.Empty);
}