using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Gangrene;

public class GangreneDryHediffComp : HediffComp
{
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (!parent.pawn.IsHashIntervalTick(GenTicks.TickRareInterval))
        {
            return;
        }
        if (parent.Severity < 1f)
        {
            float chance = GenTicks.TickRareInterval / MoreInjuriesMod.Settings.DryGangreneMeanTimeToInfection;
            if (Rand.Chance(chance))
            {
                // turn into wet gangrene with the same severity
                Pawn patient = Pawn;
                Hediff wetGangrene = HediffMaker.MakeHediff(KnownHediffDefOf.GangreneWet, patient, parent.Part);
                wetGangrene.Severity = parent.Severity;
                if (wetGangrene.TryGetComp(out HediffComp_CausedBy? causedBy))
                {
                    // copy the cause of the dry gangrene to the wet gangrene
                    causedBy!.CopyFrom(parent);
                }
                // remove dry gangrene hediff and add wet gangrene
                patient.health.RemoveHediff(parent);
                patient.health.AddHediff(wetGangrene);
            }
        }
        else
        {
            // remove limb
            Pawn patient = Pawn;
            Hediff_MissingPart missingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, patient, parent.Part);
            missingPart.lastInjury = parent.def;
            missingPart.IsFresh = true;
            // remove all hediffs from the part because it will soon be gone
            patient.health.hediffSet.RemoveHediffsMatchingPartOrChildren(missingPart.Part);
            patient.health.AddHediff(missingPart);
        }
    }
}