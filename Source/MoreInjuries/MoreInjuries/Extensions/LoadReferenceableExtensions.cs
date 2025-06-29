using Verse;

namespace MoreInjuries.Extensions;

internal static class LoadReferenceableExtensions
{
    public static string GenerateUniqueLoadID(this ILoadReferenceable loadReferencable)
    {
        _ = loadReferencable ?? throw new ArgumentNullException(nameof(loadReferencable), "loadReferencable cannot be null.");

        // Generate a unique ID based on the type and current time
        return $"MoreInjuries.{loadReferencable.GetType().Name}_{Guid.NewGuid()}";
    }
}
