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
        // f(x)=((chance_factor)/(1+e^(-4.5 (x-2.25))))
        // with increasing severity, the chance of permanent hearing loss increases
        return (float)(MoreInjuriesMod.Settings.HearingDamagePermanentChanceFactor / (1f + Mathf.Exp(-4.5f * (severity - 2.25f))));
    }

    public static bool TryMakePermanentIfApplicable(Pawn patient, Hediff temporaryHearingLoss)
    {
        if (MoreInjuriesMod.Settings.HearingDamageMayBecomePermanent && Rand.Chance(ChanceToBecomePermanent(temporaryHearingLoss.Severity)) && !patient.health.hediffSet.HasHediff(KnownHediffDefOf.HearingLoss))
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
