using System.Linq;
using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

public class ShockHediffComp : HediffComp
{
    private bool _fixedNow = false;
    private int _ticks = 0;

    private ShockHediffCompProperties Properties => (ShockHediffCompProperties)props;

    public bool PastFixedPoint => parent.Severity > 0.6f;

    private Hediff? GetBloodLoss() => parent.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);

    public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
    {
        base.CompTended(quality, maxQuality, batchPosition);

        // ensure that blood IVs can be used to stabilize the patient
        float requiredQuality = Properties.BleedSeverityCurve.Evaluate(parent.Severity);
        if (GetBloodLoss()?.Severity is null or < 0.15f || quality >= requiredQuality)
        {
            _fixedNow = true;
            Logger.LogVerbose($"Fixed hypovolemic shock for {parent.pawn.Name}");
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);

        Hediff? bloodLoss = GetBloodLoss();
        if (bloodLoss?.Severity is null or < 0.45f || _fixedNow)
        {
            // the patient is stable, start recovery
            parent.Severity -= 0.000025f;
            return;
        }
        if (!PastFixedPoint)
        {
            // scale the severity of the shock based on the severity of the blood loss
            parent.Severity = bloodLoss.Severity;
            return;
        }
        bool preventHypoxia = false;
        float maxSeverityIncrease = 0.00005f * bloodLoss.Severity;
        if (parent.IsTended())
        {
            // if the patient is tended, the severity should increase slower, with a bit of randomness
            parent.Severity += Rand.Range(0, maxSeverityIncrease);
            preventHypoxia = Rand.Chance(MoreInjuriesMod.Settings.OrganHypoxiaChanceReductionFactor);
        }
        else
        {
            parent.Severity += maxSeverityIncrease;
        }
        _ticks++;
        if (_ticks >= 300)
        {
            if (!preventHypoxia && Rand.Chance(MoreInjuriesMod.Settings.OrganHypoxiaChance))
            {
                BodyPartRecord hypoxiaTarget = parent.pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Middle, BodyPartDepth.Inside)
                    .Where(bodyPart => bodyPart.def != BodyPartDefOf.Heart
                        && bodyPart.def.bleedRate > 0f)
                    .ToList()
                    .SelectRandom();
                Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.OrganHypoxia, parent.pawn, hypoxiaTarget);
                hediff.Severity = Rand.Range(2f, 5f);
                parent.pawn.health.AddHediff(hediff, hypoxiaTarget);
            }
            // cardiac arrest chance is higher for higher blood loss
            float cardiacArrestChance = MoreInjuriesMod.Settings.CardiacArrestChanceOnHighBloodLoss * bloodLoss.Severity / 0.8f;
            if (MoreInjuriesMod.Settings.EnableCardiacArrestOnHighBloodLoss && Rand.Chance(cardiacArrestChance))
            {
                if (parent.pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Heart) is BodyPartRecord heart
                    && !parent.pawn.health.hediffSet.PartIsMissing(heart) 
                    && !parent.pawn.health.hediffSet.TryGetFirstHediffMatchingPart(heart, KnownHediffDefOf.CardiacArrest, out Hediff? cardiacArrest))
                {
                    cardiacArrest = HediffMaker.MakeHediff(KnownHediffDefOf.CardiacArrest, parent.pawn);
                    cardiacArrest.Severity = 0.01f;
                    parent.pawn.health.AddHediff(cardiacArrest, heart);
                }
            }
            _ticks = 0;
        }
    }
}
