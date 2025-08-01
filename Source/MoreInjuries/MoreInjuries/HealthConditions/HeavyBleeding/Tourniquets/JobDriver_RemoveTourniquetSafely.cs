using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public sealed class JobDriver_RemoveTourniquetSafely : JobDriver_RemoveTourniquetBase
{
    public const string JOB_LABEL_KEY = "MI_RemoveTourniquetSafely";

    // gotta take it off slowly to avoid systemic acidosis
    protected override int BaseTendDuration => Mathf.RoundToInt(Mathf.Lerp(base.BaseTendDuration, base.BaseTendDuration * 8f, IschemiaSeverity));

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, BodyPartRecord bodyPart) =>
        new JobDescriptor(KnownJobDefOf.RemoveTourniquetSafely, doctor, patient, bodyPart);
}
