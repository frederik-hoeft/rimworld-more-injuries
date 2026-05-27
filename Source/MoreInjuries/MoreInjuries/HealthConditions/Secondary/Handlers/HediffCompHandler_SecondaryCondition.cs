using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;

using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

[XmlBindable]
public abstract partial class HediffCompHandler_SecondaryCondition : HediffCompHandler_ChanceBased, IHediffCompHandler<HediffComp_SecondaryCondition>
{
    [XmlBinding("hediffMakerProps")]
    public partial HediffMakerProperties HediffMakerProps { get; }

    [XmlBinding("sendLetterWhenDiscovered")]
    public partial bool SendLetterWhenDiscovered { get; }

    [XmlBinding<bool>("applyDownstreamModifiers", defaultValue: true)]
    public partial bool ApplyDownstreamModifiers { get; }

    [XmlBinding("targetEvaluator", DefaultValueProvider = typeof(BodyPartHediffTargetEvaluator_WholeBody), DefaultValueFrom = nameof(BodyPartHediffTargetEvaluator_WholeBody.Instance))]
    public partial BodyPartHediffTargetEvaluator TargetEvaluator { get; }

    public virtual void Handle(HediffComp_SecondaryCondition comp)
    {
        HediffMakerDef hediffMakerDef = HediffMakerProps.GetHediffMakerDef(comp, handler: this);
        if (!ShouldSkip(comp, hediffMakerDef.HediffDef))
        {
            TryApplyHediff(comp, hediffMakerDef);
        }
    }

    public virtual bool ShouldSkip(HediffComp_SecondaryCondition comp, HediffDef hediffDef)
    {
        if (ShouldSkip(comp))
        {
            return true;
        }
        Hediff hediff = comp.parent;
        float chance = 1f;
        // apply modifiers from the downstream hediff
        if (ApplyDownstreamModifiers && hediffDef.GetModExtension<HediffModifier_DownstreamChanceModifiers_ModExtension>() is { } downstream)
        {
            chance *= downstream.GetModifier(hediff, compHandler: this);
        }
        return !Rand.Chance(chance);
    }

    protected virtual void TryApplyHediff(HediffComp_SecondaryCondition comp, HediffMakerDef hediffMakerDef)
    {
        HediffDef hediffDef = hediffMakerDef.HediffDef;
        BodyPartHediffTargetEvaluator localTargetEvaluator = TargetEvaluator;
        // check if the hediff already exists on the target body part (or anywhere if no body part is specified)
        BodyPartRecord? targetBodyPart = localTargetEvaluator.GetTargetBodyPart(comp, this);
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
