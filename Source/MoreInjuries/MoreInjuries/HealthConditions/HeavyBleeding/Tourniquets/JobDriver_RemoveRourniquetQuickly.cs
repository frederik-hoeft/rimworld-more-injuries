using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public sealed class JobDriver_RemoveTourniquetQuickly : JobDriver_RemoveTourniquetBase
{
    public const string JOB_LABEL_KEY = "MI_RemoveTourniquetQuickly";

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        bool success = base.ApplyDevice(doctor, patient, device);
        // quickly removing a tourniquet can cause systemic acidosis from all the gunk suddenly entering the bloodstream
        // if the tissue was only mildly ischemic, skip this (arbitrary threshold)
        if (success && IschemiaSeverity > 0.05f)
        {
            if (patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.TourniquetQuicklyRemoved, out Hediff tourniquetAcidosis))
            {
                tourniquetAcidosis.Severity += IschemiaSeverity;
            }
            else
            {
                Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.TourniquetQuicklyRemoved, patient);
                hediff.Severity = IschemiaSeverity;
                patient.health.AddHediff(hediff);
            }
        }
        return success;
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, BodyPartRecord bodyPart) =>
        new JobDescriptor(KnownJobDefOf.RemoveTourniquetQuickly, doctor, patient, bodyPart);
}
