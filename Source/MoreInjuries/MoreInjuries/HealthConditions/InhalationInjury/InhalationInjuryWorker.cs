using MoreInjuries.Defs.WellKnown;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.InhalationInjury;

internal sealed class InhalationInjuryWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableFireInhalation;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (dinfo.Def?.hediff != KnownHediffDefOf.Burn)
        {
            return;
        }
        float flammability = patient.GetStatValue(StatDefOf.Flammability);
        if (flammability <= Mathf.Epsilon || dinfo.Amount <= Mathf.Epsilon || KnownHediffDefOf.CE_WearingGasMask is { } ceGasMask && patient.health.hediffSet.HasHediff(ceGasMask))
        {
            // this pawn is immune to inhalation injuries
            return;
        }
        // defensive snapshot enumeration to avoid adding a lung burn hediff that destroys the lung and modifies the collection
        List<BodyPartRecord> lungs = [.. patient.health.hediffSet.GetNotMissingParts().Where(static bodyPart => bodyPart.def == BodyPartDefOf.Lung)];
        foreach (BodyPartRecord lung in lungs)
        {
            bool hasBurnedLung = false;
            // get burn injuries on that lung
            foreach (Hediff lungBurn in patient.health.hediffSet.hediffs)
            {
                if (lungBurn.def == KnownHediffDefOf.Burn && lungBurn.Part == lung)
                {
                    hasBurnedLung = true;
                    lungBurn.Severity += flammability * Rand.Range(0.05f, 1f);
                }
            }
            if (!hasBurnedLung)
            {
                Hediff lungBurn = HediffMaker.MakeHediff(KnownHediffDefOf.Burn, patient, lung);
                lungBurn.Severity = flammability * Rand.Range(0.05f, 1f);
                patient.health.AddHediff(lungBurn, lung);
            }
        }
    }
}