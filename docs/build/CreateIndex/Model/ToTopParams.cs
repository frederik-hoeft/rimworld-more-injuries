using System.Text.Json.Serialization;

namespace CreateIndex.Model;

[method: JsonConstructor]
public record ToTopParams
(
    [property: JsonPropertyName("template")] string? Template
);