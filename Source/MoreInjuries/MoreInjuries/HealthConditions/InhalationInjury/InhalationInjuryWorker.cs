using MoreInjuries.Defs.WellKnown;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.InhalationInjury;

internal sealed class InhalationInjuryWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableFireInhalation;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (dinfo.Def?.hediff == KnownHediffDefOf.Burn)
        {
            // defensive snapshot enumeration to avoid adding a lung burn hediff that destroys the lung and modifies the collection
            List<BodyPartRecord> lungs = [.. patient.health.hediffSet.GetNotMissingParts().Where(static bodyPart => bodyPart.def == BodyPartDefOf.Lung)];
            foreach (BodyPartRecord lung in lungs)
            {
                bool hasBurnedLung = false;
                // get burn injuries on that lung
                for (int i = 0; i < patient.health.hediffSet.hediffs.Count; ++i)
                {
                    Hediff lungBurn = patient.health.hediffSet.hediffs[i];
                    if (lungBurn.def == KnownHediffDefOf.Burn && lungBurn.Part == lung)
                    {
                        hasBurnedLung = true;
                        lungBurn.Severity += Rand.Range(0.05f, 0.5f);
                    }
                }
                if (!hasBurnedLung)
                {
                    Hediff lungBurn = HediffMaker.MakeHediff(KnownHediffDefOf.Burn, patient, lung);
                    lungBurn.Severity = Rand.Range(0.05f, 0.5f);
                    patient.health.AddHediff(lungBurn, lung);
                }
            }
        }
    }
}