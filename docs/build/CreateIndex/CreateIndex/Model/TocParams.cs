using System.Text.Json.Serialization;

namespace CreateIndex.Model;

[method: JsonConstructor]
internal sealed record TocParams
(
    [property: JsonPropertyName("source")] string Source = ".",
    [property: JsonPropertyName("indent")] int IndentSize = 4
);