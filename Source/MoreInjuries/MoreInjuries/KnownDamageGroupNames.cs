using RimWorld;
using System.Collections.Generic;
using System.Threading;

namespace MoreInjuries;

internal static class KnownDamageGroupNames
{
    public static Lazy<HashSet<string>> Explosions { get; } = new Lazy<HashSet<string>>(() =>
    [
        DamageDefOf.Bomb.defName,
        "Thermobaric"
    ], LazyThreadSafetyMode.None);
}
