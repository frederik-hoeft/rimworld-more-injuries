namespace System.Runtime.CompilerServices;

// stuff required for C# 9+ language features to make the compiler happy
internal static class IsExternalInit;

internal class RequiredMemberAttribute : Attribute;

internal class CompilerFeatureRequiredAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
}