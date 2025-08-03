using System.Text;
using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Model.Defs.SegmentBuilders;

internal sealed class StageEntrySegmentBuilder : IKeySegmentBuilder
{
    public bool TryHandleSegment(ref XElement segment, XElement defNode, List<string> path)
    {
        if (defNode is { Name.LocalName: "HediffDef" or "ThoughtDef" } 
            && segment is { Name.LocalName: "li", Parent.Name.LocalName: "stages", HasElements: true }
            && segment.Element("label") is { Value: string label })
        {
            // instead of [...].stages.li.[...], we want [...].stages.<normalized_label>.[...]
            string normalizedLabel = NormalizeHediffStageLabel(label);
            path.Add(normalizedLabel);
            return true;
        }
        return false;
    }

    private static string NormalizeHediffStageLabel(string label)
    {
        // Normalize the hediff stage label to a valid key segment
        StringBuilder normalized = new(label.Length);
        foreach (char c in label)
        {
            if (char.IsLetterOrDigit(c))
            {
                normalized.Append(c);
            }
            else if (c is '_' or ' ')
            {
                normalized.Append('_'); // Replace spaces and dashes with underscores
            }
        }
        return normalized.ToString();
    }
}