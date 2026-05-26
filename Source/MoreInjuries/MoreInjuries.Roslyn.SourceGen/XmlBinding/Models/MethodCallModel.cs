namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

/// <summary>
/// Describes a method call expression used in the getter pipeline (Transform or Validate).
/// </summary>
internal sealed record MethodCallModel(string MethodName, bool IsStatic);
