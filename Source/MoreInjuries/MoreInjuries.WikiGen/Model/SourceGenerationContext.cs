using System.Text.Json.Serialization;

namespace MoreInjuries.WikiGen.Model;

[JsonSerializable(typeof(BreadcrumbParams))]
[JsonSerializable(typeof(TocParams))]
[JsonSerializable(typeof(ToTopParams))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;