using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MoreInjuries.Debug;

internal static class DebugAssert
{
    [Conditional("DEBUG")]
    public static void NotNull(object? obj, string name)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(name);
        }
    }

    [Conditional("DEBUG")]
    public static void DefOfsAreNotNull()
    {
        List<string> nullDefOfs = typeof(DebugAssert).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<DefOf>() is not null)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(f => f.FieldType.Name.EndsWith("Def") && f.GetValue(null) is null)
            .Select(f => $"{f.DeclaringType?.Name}::{f.Name}")
            .ToList();

        if (nullDefOfs.Count > 0)
        {
            throw new InvalidOperationException($"DefOfs are null: {string.Join(", ", nullDefOfs)}");
        }
    }
}
