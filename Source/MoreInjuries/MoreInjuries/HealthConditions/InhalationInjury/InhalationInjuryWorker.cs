using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.InhalationInjury;

internal sealed class InhalationInjuryWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableFireInhalation;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Pawn;
        if (dinfo.Def?.hediff != KnownHediffDefOf.Burn || !patient.IsBurning())
        {
            return;
        }
        float flammability = patient.GetStatValue(StatDefOf.Flammability);
        float toxicResistance = Mathf.Clamp01(patient.GetStatValue(StatDefOf.ToxicEnvironmentResistance));
        if (flammability <= Mathf.Epsilon 
            || dinfo.Amount <= Mathf.Epsilon 
            || toxicResistance == 1f 
            || KnownHediffDefOf.CE_WearingGasMask is { } ceGasMask && patient.health.hediffSet.HasHediff(ceGasMask))
        {
            // this pawn is immune to inhalation injuries
            return;
        }
        toxicResistance = 1f - toxicResistance;
        // defensive snapshot enumeration to avoid adding a lung burn hediff that destroys the lung and modifies the collection
        List<BodyPartRecord> lungs = [.. patient.health.hediffSet.GetNonMissingPartsOfType(BodyPartDefOf.Lung)];
        foreach (BodyPartRecord lung in lungs)
        {
            bool hasBurnedLung = false;
            // get burn injuries on that lung (avoid IEnumerator due to potential modifications)
            for (int i = 0; i < patient.health.hediffSet.hediffs.Count; ++i)
            {
                Hediff lungBurn = patient.health.hediffSet.hediffs[i];
                if (lungBurn.def == KnownHediffDefOf.Burn && lungBurn.Part == lung)
                {
                    hasBurnedLung = true;
                    lungBurn.Severity += toxicResistance * flammability * Rand.Range(0.05f, 1f);
                }
            }
            if (!hasBurnedLung)
            {
                Hediff lungBurn = HediffMaker.MakeHediff(KnownHediffDefOf.Burn, patient, lung);
                lungBurn.Severity = toxicResistance * flammability * Rand.Range(0.05f, 1f);
                patient.health.AddHediff(lungBurn, lung);
            }
        }
    }
}