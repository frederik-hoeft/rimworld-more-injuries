namespace MoreInjuries.Roslyn.Metadata.Settings;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SettingsEntryAttribute<T> : Attribute
{
    /// <summary>
    /// The name of the settings entry.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The default value for the settings entry.
    /// </summary>
    public T? DefaultValue { get; set; }
}
