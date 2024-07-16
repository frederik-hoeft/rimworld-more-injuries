namespace System.Runtime.CompilerServices;

internal static class IsExternalInit;

internal class RequiredMemberAttribute : Attribute;

internal class CompilerFeatureRequiredAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
}