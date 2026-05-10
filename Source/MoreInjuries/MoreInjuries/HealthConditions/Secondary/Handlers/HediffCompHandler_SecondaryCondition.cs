using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

[XmlBindable]
public abstract partial class HediffCompHandler_SecondaryCondition : HediffCompHandler
{
    [XmlBinding<float>("baseChance", defaultValue: 1f)]
    public virtual partial float BaseChance { get; }

    [XmlBinding("hediffMakerProps")]
    public partial HediffMakerProperties? HediffMakerProps { get; }

    [XmlBinding("chanceModifiers")]
    public partial IReadOnlyList<SecondaryHediffModifier>? ChanceModifiers { get; }

    [XmlBinding("sendLetterWhenDiscovered")]
    public partial bool SendLetterWhenDiscovered { get; }

    [XmlBinding("targetEvaluator", DefaultValueProvider = typeof(BodyPartHediffTargetEvaluator_WholeBody), DefaultValueFrom = nameof(BodyPartHediffTargetEvaluator_WholeBody.Instance))]
    public partial BodyPartHediffTargetEvaluator TargetEvaluator { get; }

    public virtual bool ShouldSkip(HediffComp_SecondaryCondition comp)
    {
        if (comp.Pawn.Dead)
        {
            return true;
        }
        if (comp.SeverityCurve is not null && !Rand.Chance(comp.SeverityCurve.Evaluate(comp.parent.Severity)))
        {
            return true;
        }
        float chance = BaseChance;
        if (chance > Mathf.Epsilon && ChanceModifiers is { Count: > 0 })
        {
            foreach (SecondaryHediffModifier modifier in ChanceModifiers)
            {
                chance *= modifier.GetModifier(comp.parent, this);
                if (chance <= Mathf.Epsilon)
                {
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

    protected virtual void Evaulate(HediffComp_SecondaryCondition comp)
    {
        Throw.InvalidOperationException.IfNull(this, HediffMakerProps);

        BodyPartHediffTargetEvaluator localTargetEvaluator = TargetEvaluator;
        BodyPartRecord? targetBodyPart = localTargetEvaluator.GetTargetBodyPart(comp, this);
        HediffMakerDef hediffMakerDef = HediffMakerProps.GetHediffMakerDef(comp, handler: this, targetBodyPart);
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

    protected virtual Hediff MakeHediff(HediffComp_SecondaryCondition sourceComp, HediffDef hediffDef, BodyPartRecord? targetBodyPart, float severity)
    {
        Hediff hediff = HediffMaker.MakeHediff(hediffDef, sourceComp.Pawn);
        hediff.Severity = severity;
        UpdateHediffCause(hediff, sourceComp);
        sourceComp.Pawn.health.AddHediff(hediff, targetBodyPart);
        return hediff;
    }

    protected virtual void PostApplyHediff(HediffComp_SecondaryCondition comp, Hediff hediff)
    {
        Pawn pawn = hediff.pawn;
        if (SendLetterWhenDiscovered && PawnUtility.ShouldSendNotificationAbout(pawn))
        {
            Find.LetterStack.ReceiveLetter("LetterHealthComplicationsLabel".Translate(pawn.LabelShort, hediff.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(),
                "LetterHealthComplications".Translate(pawn.LabelShortCap, hediff.LabelCap, comp.parent.LabelCap, pawn.Named("PAWN")).CapitalizeFirst(),
                LetterDefOf.NegativeEvent, pawn);
        }
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
