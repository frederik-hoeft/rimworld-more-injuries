using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

public class LungCollapseWorker(InjuryComp parent) : InjuryWorker(parent), IPostPreApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.lungcollapse;

    public void PostPreApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (dinfo.Def == DamageDefOf.Bomb || dinfo.Def.defName is "Thermobaric")
        {
            IEnumerable<BodyPartRecord> lungs = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def == BodyPartDefOf.Lung);

            foreach (BodyPartRecord lung in lungs)
            {
                Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.LungCollapse, patient, lung);
                hediff.Severity = Rand.Range(1f, Mathf.Max(lung.def.hitPoints * 0.75f, 1f));
                patient.health.AddHediff(hediff, lung);
            }
        }
    }
}
