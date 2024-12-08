using MoreInjuries.KnownDefs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

internal static class HearingLossHelper
{
    public static float ChanceToBecomePermanent(float severity)
    {
        // f(x)=((0.5)/(1+e^(-10 (x-1.25))))
        // with increasing severity, the chance of permanent hearing loss increases
        return (float)(0.5f / (1f + Mathf.Exp(-10f * (severity - 1.25f))));
    }

    public static bool TryMakePermanentIfApplicable(Pawn patient, Hediff temporaryHearingLoss)
    {
        if (Rand.Chance(ChanceToBecomePermanent(temporaryHearingLoss.Severity)) && !patient.health.hediffSet.HasHediff(KnownHediffDefOf.HearingLoss))
        {
            IEnumerable<BodyPartRecord> ears = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def == KnownBodyPartDefOf.Ear);
            foreach (BodyPartRecord ear in ears)
            {
                Hediff? hearingLossPermanent = HediffMaker.MakeHediff(KnownHediffDefOf.HearingLoss, patient, ear);
                patient.health.AddHediff(hearingLossPermanent);
            }
            return true;
        }
        return false;
    }
}
