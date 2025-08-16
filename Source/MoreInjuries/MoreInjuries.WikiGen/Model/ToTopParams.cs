using System.Text.Json.Serialization;

namespace MoreInjuries.WikiGen.Model;

[method: JsonConstructor]
internal sealed record ToTopParams
(
    [property: JsonPropertyName("template")] string? Template
);