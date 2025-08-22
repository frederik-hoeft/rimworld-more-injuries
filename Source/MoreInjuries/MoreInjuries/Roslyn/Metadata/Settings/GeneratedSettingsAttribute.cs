namespace MoreInjuries.Roslyn.Metadata.Settings;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GeneratedSettingsAttribute : Attribute
{
    public string? ExposeData { get; set; }
}