using System.Collections.Frozen;
using System.Xml.Linq;

namespace MoreInjuries.LocalizationTests.Model.Defs;

internal sealed class DefDatabase
{
    private static readonly FrozenSet<string> s_localizableFields =
    [
        "label",
        "description",
        "reportString",
        "labelNoun",
        "customLabel",
        "message",
        "labelTendedWell",
        "labelTendedWellInner",
        "labelSolidTendedWell",
        "permanentLabel",
        "destroyedLabel",
        "destroyedOutLabel",
        "jobString",
        "successfullyRemovedHediffMessage",
        "generalTitle",
        "generalDescription",
        "ingestCommandString",
        "ingestReportString",
        "verb",
        "gerund",
        "letterLabel",
        "letterText",
    ];

    private bool _isLoaded = false;

    public Dictionary<string, Dictionary<string, LocalizationValue>> AllDefs { get; } = [];

    public void Load(DirectoryInfo defsRoot, LoadErrorContext errorContext)
    {
        Assert.IsFalse(_isLoaded, "DefDatabase has already been loaded.");
        _isLoaded = true;

        int parentPathLength = defsRoot.FullName.Length + 1;
        foreach (FileInfo file in defsRoot.EnumerateFiles("*.xml", SearchOption.AllDirectories))
        {
            string relativePath = file.FullName[parentPathLength..];
            DefFileLoadContext context = new(relativePath, file, errorContext);
            LoadFile(context);
        }
    }

    private void LoadFile(DefFileLoadContext context)
    {
        using FileStream stream = context.SourceFile.OpenRead();
        XDocument document = XDocument.Load(stream);
        for (XNode? root = document.Root; root is not null; root = root.NextNode)
        {
            if (root is XElement { Name.LocalName: "Defs" } defs)
            {
                LoadDefs(defs, context);
            }
        }
    }

    private void LoadDefs(XElement defsRoot, DefFileLoadContext context)
    {
        foreach (XElement defNode in defsRoot.Elements())
        {
            string typedDef = defNode.Name.LocalName;
            if (!AllDefs.TryGetValue(typedDef, out Dictionary<string, LocalizationValue>? typedDefs))
            {
                typedDefs = [];
                AllDefs[typedDef] = typedDefs;
            }
            LoadDef(defNode, typedDefs, context);
        }
    }

    private static void LoadDef(XElement defNode, Dictionary<string, LocalizationValue> typedDefs, DefFileLoadContext context)
    {
        string? defName = defNode.Element("defName")?.Value;
        if (string.IsNullOrEmpty(defName))
        {
            if (defNode.Attribute("Abstract") is not { Value: ['T' or 't', 'r', 'u', 'e'] })
            {
                context.ReportMissingDefName(defNode);
            }
            return;
        }
        foreach (XElement field in GetLocalizableFields(defNode))
        {
            string key = DefKeyBuilder.CreateKey(field, defNode, defName);
            if (typedDefs.TryGetValue(key, out LocalizationValue? existingValue))
            {
                context.ReportDuplicateKeyFor(field, existingValue);
                continue;
            }
            string? value = field.Value;
            if (string.IsNullOrEmpty(value))
            {
                context.ErrorContext.Errors.Add($"[{context.RelativePath}]: Field '{field.Name.LocalName}' in def '{defName}' is empty.");
                continue;
            }
            typedDefs[key] = new LocalizationValue(key, context.RelativePath, value, null);
        }
    }

    private static IEnumerable<XElement> GetLocalizableFields(XElement defNode)
    {
        foreach (XElement child in defNode.Elements())
        {
            if (child.HasElements)
            {
                foreach (XElement subField in GetLocalizableFields(child))
                {
                    yield return subField;
                }
            }
            else if (s_localizableFields.Contains(child.Name.LocalName))
            {
                yield return child;
            }
        }
    }
}