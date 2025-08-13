using System.Text.Json.Serialization;

namespace CreateIndex.Model;

[method: JsonConstructor]
internal sealed record BreadcrumbParams
(
    [property: JsonPropertyName("root")] string? Root,
    [property: JsonPropertyName("template")] string? Template,
    [property: JsonPropertyName("connector")] string? Connector
);