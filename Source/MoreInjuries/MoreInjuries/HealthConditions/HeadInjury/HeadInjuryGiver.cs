using MoreInjuries.Extensions;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury;

public abstract class HeadInjuryGiver : IInjuryHandler
{
    public abstract bool IsEnabled { get; }

    protected abstract float SettingsMaximumEquivalentDamageSkull { get; }

    protected abstract float SettingsChance { get; }

    protected abstract HediffDef HediffDef { get; }

    protected abstract float CalculateSeverity(float equivalentLikeliness);

    public void TryGiveInjury(Pawn patient, float equivalentHeadTrauma)
    {
        float equivalentLikeliness = equivalentHeadTrauma / SettingsMaximumEquivalentDamageSkull;
        float equivalentLikelinessClamped = Mathf.Clamp01(equivalentLikeliness);
        // scale everything accordingly to the defined chance
        float adjustedChance = equivalentLikelinessClamped * SettingsChance;
        if (Rand.Chance(adjustedChance) && patient.health.hediffSet.GetBrain() is BodyPartRecord brain)
        {
            if (!patient.health.hediffSet.TryGetFirstHediffMatchingPart(brain, HediffDef, out Hediff? trauma))
            {
                trauma = HediffMaker.MakeHediff(HediffDef, patient);
                patient.health.AddHediff(trauma, brain);
            }
            float severity = CalculateSeverity(equivalentLikeliness);
            trauma!.Severity += severity;
        }
    }
}
