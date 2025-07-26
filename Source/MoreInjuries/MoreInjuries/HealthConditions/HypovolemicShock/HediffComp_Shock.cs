using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.HeavyBleeding;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

using static BloodLossConstants;
using HediffInfo = (Hediff? BloodLoss, Hediff? AdrenalineRush);

public class HediffComp_Shock : HediffComp
{
    private const int CYCLE_LENGTH = 150;

    private bool _fixedNow = false;

    private HediffCompProperties_Shock Properties => (HediffCompProperties_Shock)props;

    public bool PastFixedPoint => parent.Severity > 0.6f;

    public bool FixedNow 
    { 
        get => _fixedNow; 
        set => _fixedNow = value; 
    }

    private Hediff? GetBloodLoss() => parent.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);

    private static HediffInfo GetHediffInfo(Pawn pawn)
    {
        HediffInfo info = default;
        int hediffsFound = 0;
        for (int i = 0; i < pawn.health.hediffSet.hediffs.Count && hediffsFound < 2; ++i)
        {
            Hediff hediff = pawn.health.hediffSet.hediffs[i];
            if (hediff.def == KnownHediffDefOf.AdrenalineRush)
            {
                info.AdrenalineRush = hediff;
                hediffsFound++;
            }
            else if (hediff.def == HediffDefOf.BloodLoss)
            {
                info.BloodLoss = hediff;
                hediffsFound++;
            }
        }
        return info;
    }

    public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
    {
        base.CompTended(quality, maxQuality, batchPosition);

        // ensure that blood IVs can be used to stabilize the patient
        float requiredQuality = Properties.BleedSeverityCurve.Evaluate(parent.Severity);
        if (GetBloodLoss()?.Severity is null or < 0.15f || quality >= requiredQuality)
        {
            FixedNow = true;
            Logger.LogVerbose($"Fixed hypovolemic shock for {parent.pawn.Name}");
        }
    }

    public override void CompExposeData()
    {
        base.CompExposeData();

        Scribe_Values.Look(ref _fixedNow, "fixedNow", false);
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        Pawn pawn = parent.pawn;
        if (!pawn.IsHashIntervalTick(CYCLE_LENGTH))
        {
            return;
        }
        (Hediff? bloodLoss, Hediff? adrenaline) = GetHediffInfo(pawn);
        // adrenaline increases blood pressure, which can offset the severity of the shock a bit,
        // at max 1.5x the normal recovery rate or 1/1.5 = 0.67x the normal increase rate
        float adrenalineBloodPressureOffset = Mathf.Clamp01((adrenaline?.Severity ?? 0f) / 2f) + 1f;
        if (bloodLoss is not { Severity: > BLOOD_LOSS_THRESHOLD } || FixedNow)
        {
            // the patient is stable, start recovery
            parent.Severity -= 0.00375f * adrenalineBloodPressureOffset;
            return;
        }
        if (!PastFixedPoint)
        {
            // scale the severity of the shock based on the severity of the blood loss
            parent.Severity = bloodLoss.Severity;
            return;
        }
        if (bloodLoss is null)
        {
            return;
        }
        float maxSeverityIncrease = 0.0075f * bloodLoss.Severity / adrenalineBloodPressureOffset;
        if (parent.IsTended())
        {
            // if the patient is tended, the severity should increase slower, with a bit of randomness
            parent.Severity += Rand.Range(0, maxSeverityIncrease);
        }
        else
        {
            parent.Severity += maxSeverityIncrease;
        }
    }
}
