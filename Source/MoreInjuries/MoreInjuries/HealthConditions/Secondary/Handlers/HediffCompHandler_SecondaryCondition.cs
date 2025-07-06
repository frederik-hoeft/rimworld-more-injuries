using MoreInjuries.BuildIntrinsics;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffCompHandler_SecondaryCondition : HediffCompHandler
{
    // don't rename this field. XML defs depend on this name
    protected readonly float? baseChance = null;
    // don't rename this field. XML defs depend on this name
    private readonly int tickInterval = GenTicks.TickRareInterval;
    // don't rename this field. XML defs depend on this name
    private readonly HediffMakerProperties? hediffMakerProps = default;
    // don't rename this field. XML defs depend on this name
    private readonly List<SecondaryHediffModifier>? chanceModifiers = default;
    // don't rename this field. XML defs depend on this name
    protected readonly bool sendLetterWhenDiscovered = false;
    // don't rename this field. XML defs depend on this name
    protected readonly BodyPartHediffTargetEvaluator? targetEvaluator = default;

    public virtual float BaseChance => baseChance ?? 1f;

    public override int TickInterval => tickInterval;

    public HediffMakerProperties? HediffMakerProps => hediffMakerProps;

    public virtual bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        if (!comp.Pawn.IsHashIntervalTick(TickInterval) || comp.Pawn.Dead)
        {
            return true;
        }
        if (comp.SeverityCurve is not null && !Rand.Chance(comp.SeverityCurve.Evaluate(comp.parent.Severity)))
        {
            return true;
        }
        float chance = BaseChance;
        if (chance > Mathf.Epsilon && chanceModifiers is { Count: > 0 })
        {
            foreach (SecondaryHediffModifier modifier in chanceModifiers)
            {
                chance *= modifier.GetModifier(comp.parent, this);
                if (chance <= Mathf.Epsilon)
                {
                    Logger.LogDebug($"Chance for {comp.Pawn.Name} ({comp.parent.LabelCap}) to get the secondary condition is 0. Skipping evaluation.");
                    return true;
                }
            }
        }
        if (chance <= Mathf.Epsilon || chance < 1f && !Rand.Chance(chance))
        {
            return true;
        }
        return false;
    }

    public virtual void Evaluate(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        if (ShouldSkip(comp, severityAdjustment))
        {
            return;
        }
        BodyPartHediffTargetEvaluator localTargetEvaluator = targetEvaluator ?? BodyPartHediffTargetEvaluator_WholeBody.Instance;
        HediffMakerProperties hediffMakerProps = HediffMakerProps ?? throw new InvalidOperationException($"{nameof(HediffCompHandler_SecondaryCondition)}: {comp.GetType().Name} has no hediff maker properties defined. Cannot evaluate.");
        BodyPartRecord? targetBodyPart = localTargetEvaluator.GetTargetBodyPart(comp, this);
        HediffMakerDef hediffMakerDef = hediffMakerProps.GetHediffMakerDef(comp, handler: this, targetBodyPart);
        HediffDef hediffDef = hediffMakerDef.HediffDef;
        // check if the hediff already exists on the target body part (or anywhere if no body part is specified)
        Hediff? existingHediff = null;
        if (targetBodyPart is null && !comp.Pawn.health.hediffSet.TryGetHediff(hediffDef, out existingHediff) 
            || targetBodyPart is not null && !comp.Pawn.health.hediffSet.TryGetFirstHediffMatchingPart(targetBodyPart, hediffDef, out existingHediff))
        {
            float initialSeverity = hediffMakerDef.GetInitialSeverity();
            Hediff hediff = MakeHediff(comp, hediffDef, targetBodyPart, initialSeverity);
            PostApplyHediff(comp, hediff);
            return;
        }
        if (!hediffMakerDef.AllowMultiple)
        {
            // if the hediff already exists, we don't need to create a new one
            Logger.LogDebug($"Hediff {hediffDef.defName} already exists for {comp.Pawn.Name} ({comp.parent.LabelCap}). Skipping creation.");
            return;
        }
        if (hediffMakerDef.AllowDuplicate)
        {
            // if duplicates are allowed, we can create a new hediff even if it already exists
            float initialSeverity = hediffMakerDef.GetInitialSeverity();
            Hediff hediff = MakeHediff(comp, hediffDef, targetBodyPart, initialSeverity);
            PostApplyHediff(comp, hediff);
        }
        else if (existingHediff is not null)
        {
            // otherwise, we update the existing hediff's severity
            UpdateHediffCause(existingHediff, comp);
            existingHediff.Severity += hediffMakerDef.GetInitialSeverity();
        }
    }

    protected virtual void PostApplyHediff(HediffComp_SecondaryCondition comp, Hediff hediff)
    {
        Pawn pawn = hediff.pawn;
        if (sendLetterWhenDiscovered && PawnUtility.ShouldSendNotificationAbout(pawn))
        {
            Find.LetterStack.ReceiveLetter("LetterHealthComplicationsLabel".Translate(pawn.LabelShort, hediff.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(),
                "LetterHealthComplications".Translate(pawn.LabelShortCap, hediff.LabelCap, comp.parent.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(),
                LetterDefOf.NegativeEvent, pawn);
        }
    }

    protected virtual Hediff MakeHediff(HediffComp_SecondaryCondition sourceComp, HediffDef hediffDef, BodyPartRecord? targetBodyPart, float severity)
    {
        Hediff hediff = HediffMaker.MakeHediff(hediffDef, sourceComp.Pawn);
        hediff.Severity = severity;
        UpdateHediffCause(hediff, sourceComp);
        sourceComp.Pawn.health.AddHediff(hediff, targetBodyPart);
        return hediff;
    }

    protected virtual void UpdateHediffCause(Hediff hediff, HediffComp_SecondaryCondition sourceComp)
    {
        if (hediff is not HediffWithComps { comps.Count: > 0 } hediffWithComps)
        {
            return;
        }
        foreach (HediffComp? comp in hediffWithComps.comps)
        {
            if (comp is HediffComp_CausedBy compCausedBy)
            {
                // update the CausedBy property to the reason this hediff was created
                compCausedBy.AddCause(sourceComp.parent);
                return;
            }
        }
    }
}