namespace MoreInjuries.Roslyn.SourceGen.Contracts;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
internal sealed class GeneratorContractRegistrationAttribute<T>(T type) : Attribute where T : unmanaged, Enum
{
    public T Type { get; } = type;
}