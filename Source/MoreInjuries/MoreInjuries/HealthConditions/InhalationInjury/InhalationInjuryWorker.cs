using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.InhalationInjury;

internal class InhalationInjuryWorker(InjuryComp parent) : InjuryWorker(parent), IPostPreApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableFireInhalation;

    public void PostPreApplyDamage(ref readonly DamageInfo dinfo)
    {
        HediffDef burnHediffDef = DamageDefOf.Burn.hediff;
        Pawn patient = Target;
        if (dinfo.Def.hediff == burnHediffDef)
        {
            IEnumerable<BodyPartRecord> lungs = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def == BodyPartDefOf.Lung);
            foreach (BodyPartRecord lung in lungs)
            {
                bool hasBurnedLung = false;
                // get burn injuries on that lung
                foreach (Hediff lungBurn in patient.health.hediffSet.hediffs.Where(hediff => hediff.Part == lung && hediff.def == burnHediffDef))
                {
                    hasBurnedLung = true;
                    lungBurn.Severity += 8f;
                }
                if (!hasBurnedLung)
                {
                    Hediff lungBurn = HediffMaker.MakeHediff(burnHediffDef, patient, lung);
                    lungBurn.Severity = 200f;
                    patient.health.AddHediff(lungBurn, lung);
                }
            }
        }
    }
}
