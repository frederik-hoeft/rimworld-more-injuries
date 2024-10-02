using MoreInjuries.KnownDefs;
using RimWorld;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.EmpShutdown;

internal class EmpBionicsWorker(InjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableEmpDamageToBionics;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;

        if (dinfo.Def == DamageDefOf.EMP || dinfo.Def == DamageDefOf.ElectricalBurn)
        {
            // make a snapshot of the bionic hediffs to avoid modifying the collection while iterating
            Hediff[] bionicHediffSnapshot = [.. patient.health.hediffSet.hediffs.Where(hediff => hediff is { Part: not null, def.addedPartProps.betterThanNatural: true })];
            float chance = MoreInjuriesMod.Settings.EmpDamageToBionicsChance;
            foreach (Hediff part in bionicHediffSnapshot)
            {
                if (Rand.Chance(chance))
                {
                    Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.EmpShutdown, patient, part.Part);
                    hediff.Severity = 1f;
                    patient.health.AddHediff(hediff, part.Part);
                }
            }
        }
    }
}
