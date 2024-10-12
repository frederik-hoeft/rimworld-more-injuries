using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Gangrene;

public class GangreneDryHediffComp : HediffComp
{
    private const int CHECK_INTERVAL = 300;
    private int _ticksToNextCheck = CHECK_INTERVAL;

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);

        if (--_ticksToNextCheck <= 0)
        {
            _ticksToNextCheck = CHECK_INTERVAL;
            if (parent.Severity < 1f)
            {
                float chance = CHECK_INTERVAL / MoreInjuriesMod.Settings.DryGangreneMeanTimeToInfection;
                if (Rand.Chance(chance))
                {
                    // turn into wet gangrene with the same severity
                    Pawn patient = Pawn;
                    Hediff wetGangrene = HediffMaker.MakeHediff(KnownHediffDefOf.GangreneWet, patient, parent.Part);
                    wetGangrene.Severity = parent.Severity;
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
}