using System.Text.Json.Serialization;

namespace CreateIndex.Model;

[method: JsonConstructor]
internal sealed record ToTopParams
(
    [property: JsonPropertyName("template")] string? Template
);