using MoreInjuries.LocalizationTests.Model.Defs.SegmentBuilders;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Model.Defs;

internal static class DefKeyBuilder
{
    private static readonly ImmutableArray<IKeySegmentBuilder> s_keySegmentBuilderPipeline =
    [
        new StageEntrySegmentBuilder(),
        new CompEntrySegmentBuilder(),
        new DefaultSegmentBuilder(),
    ];

    public static string CreateKey(XElement field, XElement defNode, string defName)
    {
        List<string> path = [];
        XElement? current;
        for (current = field; current is not null && current != defNode; current = current.Parent)
        {
            bool handled = false;
            foreach (IKeySegmentBuilder segmentBuilder in s_keySegmentBuilderPipeline)
            {
                if (segmentBuilder.TryHandleSegment(ref current, defNode, path))
                {
                    handled = true;
                    break;
                }
            }
            if (!handled)
            {
                throw new InvalidOperationException($"Unable to handle segment '{current.Name.LocalName}' in path for field '{field.Name.LocalName}'.");
            }
        }
        if (current != defNode)
        {
            throw new InvalidOperationException($"Field '{field.Name.LocalName}' is not a child of the def node.");
        }
        StringBuilder keyBuilder = new(defName);
        for (int i = path.Count - 1; i >= 0; --i)
        {
            string element = path[i];
            keyBuilder.Append('.').Append(element);
        }
        return keyBuilder.ToString();
    }
}
