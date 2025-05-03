// shims for modern compiler features in legacy .NET Framework

namespace System.Runtime.CompilerServices;

internal class CompilerFeatureRequiredAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
}
