#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Runtime.CompilerServices;
#pragma warning restore IDE0130 // Namespace does not match folder structure

// stuff required for modern C# language features to make the compiler happy
internal static class IsExternalInit;

internal class RequiredMemberAttribute : Attribute;

internal class CompilerFeatureRequiredAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
}
