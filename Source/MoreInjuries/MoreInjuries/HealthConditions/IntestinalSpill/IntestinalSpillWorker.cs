using MoreInjuries.Bcl;
using MoreInjuries.KnownDefs;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.IntestinalSpill;

internal class IntestinalSpillWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    private static readonly HashSet<BodyPartDef> s_afflictedOrgans =
    [
        KnownBodyPartDefOf.SmallIntestine,
        KnownBodyPartDefOf.LargeIntestine,
        KnownBodyPartDefOf.Stomach,
        KnownBodyPartDefOf.Kidney,
        KnownBodyPartDefOf.Liver
    ];

    public override bool IsEnabled => true;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn targetPawn = Target;
        if (damage.parts is null)
        {
            return;
        }

        if (damage.parts.Any(bodyPart =>
            bodyPart.def == KnownBodyPartDefOf.SmallIntestine
            || bodyPart.def == KnownBodyPartDefOf.LargeIntestine
            || bodyPart.def == KnownBodyPartDefOf.Stomach))
        {
            float chance = MoreInjuriesMod.Settings.IntestinalSpillingChanceOnDamage;
            foreach (BodyPartRecord bodyPart in targetPawn.health.hediffSet.GetNotMissingParts())
            {
                // if we have spillage from the intestines and any of the affected organs are bleeding, there's a chance to cause acid burns
                if (s_afflictedOrgans.Contains(bodyPart.def)
                    && targetPawn.health.hediffSet.hediffs.Any(hediff => hediff is { Part: not null, Bleeding: true } && hediff.Part == bodyPart)
                    && Rand.Chance(chance))
                {
                    Hediff burn = HediffMaker.MakeHediff(KnownHediffDefOf.StomachAcidBurn, targetPawn, bodyPart);
                    burn.Severity = Rand.Range(1, 7f);
                    targetPawn.health.AddHediff(burn);
                }
            }
        }
    }
}
