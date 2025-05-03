using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.InhalationInjury;

internal class InhalationInjuryWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
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
                foreach (Hediff lungBurn in patient.health.hediffSet.hediffs.Where(hediff => hediff.Part == lung && hediff.def == KnownHediffDefOf.Burn))
                {
                    hasBurnedLung = true;
                    lungBurn.Severity += Rand.Range(0.05f, 0.5f);
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
