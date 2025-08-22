namespace MoreInjuries.Roslyn.CompilerServices;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class CompileTimePlatformInfoAttribute : Attribute;
