using MoreInjuries.Caching;
using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

internal readonly struct BleedRateByLimbEnumerable : IEnumerable<KeyValuePair<BodyPartRecord, float>>, IDisposable
{
    private static readonly ObjectPool<Dictionary<BodyPartRecord, float>> s_bleedRateCaches = new(maxCapacity: 4, factory: static _ => new Dictionary<BodyPartRecord, float>(capacity: 5));

    private readonly Dictionary<BodyPartRecord, float> _cache;

    private BleedRateByLimbEnumerable(Dictionary<BodyPartRecord, float> cache) => _cache = cache;

    private static IEnumerable<BodyPartRecord> GetLimbs(Pawn patient) => patient.health.hediffSet.GetNotMissingParts()
        .Where(bodyPart => (bodyPart.def == BodyPartDefOf.Shoulder
            || bodyPart.def == BodyPartDefOf.Leg
            // a nice little easter egg for the less-gifted doctors out there :)
            || bodyPart.def == KnownBodyPartDefOf.Neck)
            && !bodyPart.def.IsSolid(bodyPart, patient.health.hediffSet.hediffs));

    public static BleedRateByLimbEnumerable EvaluateLimbs(Pawn patient)
    {
        Dictionary<BodyPartRecord, float> bleedRateByLimbCache = s_bleedRateCaches.Rent();
        // populate bleed rate cache
        foreach (BodyPartRecord bodyPart in GetLimbs(patient))
        {
            bleedRateByLimbCache.TryAdd(bodyPart, 0f);
        }
        // calculate bleed rate for each limb
        foreach (Hediff hediff in patient.health.hediffSet.hediffs)
        {
            // tourniquets can only be applied to bleeding injuries that are tendable
            if (hediff is HediffWithComps { Bleeding: true } && hediff.TendableNow()
                // and the injury must be on the targeted body part or one of its children
                && hediff.TryFindMatchingBodyPart(bleedRateByLimbCache, out BodyPartRecord? matchingPart, out float partBleedRate))
            {
                bleedRateByLimbCache[matchingPart] = partBleedRate + hediff.BleedRate;
            }
        }
        return new BleedRateByLimbEnumerable(bleedRateByLimbCache);
    }

    public IEnumerator<KeyValuePair<BodyPartRecord, float>> GetEnumerator() => _cache.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        _cache.Clear();
        s_bleedRateCaches.Return(_cache);
    }
}