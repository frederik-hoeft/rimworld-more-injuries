using System.Linq;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.HeavyBleeding;
using MoreInjuries.KnownDefs;
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
    private int _ticks = 0;
    private int _cycles = 0;

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
        base.CompPostTick(ref severityAdjustment);

        if (++_ticks < CYCLE_LENGTH)
        {
            return;
        }
        _ticks = 0;
        Pawn pawn = parent.pawn;
        (Hediff? bloodLoss, Hediff? adrenaline) = GetHediffInfo(pawn);
        // adrenaline increases blood pressure, which can offset the severity of the shock a bit,
        // at max 1.5x the normal recovery rate or 1/1.5 = 0.67x the normal increase rate
        float adrenalineBloodPressureOffset = Mathf.Clamp01((adrenaline?.Severity ?? 0f) / 2f) + 1f;
        bool severityCalculationFinished = false;
        if (bloodLoss?.Severity is null or < BLOOD_LOSS_THRESHOLD || FixedNow)
        {
            // the patient is stable, start recovery
            parent.Severity -= 0.00375f * adrenalineBloodPressureOffset;
            severityCalculationFinished = true;
        }
        if (!PastFixedPoint)
        {
            if (!severityCalculationFinished)
            {
                // scale the severity of the shock based on the severity of the blood loss
                parent.Severity = bloodLoss!.Severity;
            }
            return;
        }
        if (bloodLoss is null)
        {
            return;
        }
        bool preventHypoxia = false;
        if (!severityCalculationFinished)
        {
            float maxSeverityIncrease = 0.0075f * bloodLoss.Severity / adrenalineBloodPressureOffset;
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
        }
        // run more expensive hypoxia and cardiac arrest checks every 2 cycles (300 ticks = 5 seconds)
        if (++_cycles < 2)
        {
            return;
        }
        _cycles = 0;
        if (!preventHypoxia && Rand.Chance(MoreInjuriesMod.Settings.OrganHypoxiaChance))
        {
            BodyPartRecord hypoxiaTarget = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Middle, BodyPartDepth.Inside)
                .Where(bodyPart => bodyPart.def != BodyPartDefOf.Heart
                    && bodyPart.def.bleedRate > 0f)
                .ToList()
                .SelectRandom();
            Hediff hediff = HediffMaker.MakeHediff(KnownHediffDefOf.OrganHypoxia, pawn, hypoxiaTarget);
            hediff.Severity = Rand.Range(2f, 5f);
            pawn.health.AddHediff(hediff, hypoxiaTarget);
        }
        // cardiac arrest chance is higher for higher blood loss
        float cardiacArrestChance = MoreInjuriesMod.Settings.CardiacArrestChanceOnHighBloodLoss * bloodLoss.Severity / 0.8f;
        if (MoreInjuriesMod.Settings.EnableCardiacArrestOnHighBloodLoss && Rand.Chance(cardiacArrestChance)
            // Biotech integration: don't apply cardiac arrest if the pawn is deathresting, otherwise leads to infinite cardiac arrest
            && (!ModLister.BiotechInstalled || !parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest)))
        {
            if (pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Heart) is BodyPartRecord heart
                && !pawn.health.hediffSet.PartIsMissing(heart)
                && !pawn.health.hediffSet.TryGetFirstHediffMatchingPart(heart, KnownHediffDefOf.CardiacArrest, out Hediff? cardiacArrest))
            {
                cardiacArrest = HediffMaker.MakeHediff(KnownHediffDefOf.CardiacArrest, parent.pawn);
                cardiacArrest.Severity = 0.01f;
                parent.pawn.health.AddHediff(cardiacArrest, heart);
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    Find.LetterStack.ReceiveLetter("LetterHealthComplicationsLabel".Translate(pawn.LabelShort, cardiacArrest.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(), 
                        "LetterHealthComplications".Translate(pawn.LabelShortCap, cardiacArrest.LabelCap, parent.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(), 
                        LetterDefOf.NegativeEvent, pawn);
                }
            }
        }
        _ticks = 0;
    }
}
