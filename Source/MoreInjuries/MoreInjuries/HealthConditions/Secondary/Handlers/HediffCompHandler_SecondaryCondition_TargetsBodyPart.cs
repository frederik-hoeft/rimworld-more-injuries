using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffCompHandler_SecondaryCondition_TargetsBodyPart : HediffCompHandler_SecondaryCondition
{
    // don't rename this field. XML defs depend on this name
    protected readonly BodyPartHediffTargetEvaluator targetEvaluator = default!;

    public override void Evaluate(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        if (ShouldSkip(comp, severityAdjustment))
        {
            return;
        }
        _ = targetEvaluator ?? throw new InvalidOperationException($"{nameof(HediffCompHandler_SecondaryCondition_TargetsBodyPart)}: {comp.GetType().Name} has no target evaluator defined. Cannot evaluate.");
        HediffMakerProperties hediffMakerProps = HediffMakerProps ?? throw new InvalidOperationException($"{nameof(HediffCompHandler_SecondaryCondition_TargetsBodyPart)}: {comp.GetType().Name} has no hediff maker properties defined. Cannot evaluate.");
        BodyPartRecord? targetBodyPart = targetEvaluator.GetTargetBodyPart(comp, this);
        if (targetBodyPart is null)
        {
            Logger.LogDebug($"No valid target body part found for {comp.Pawn.Name} ({comp.parent.LabelCap}). Skipping evaluation.");
            return;
        }
        HediffMakerDef hediffMakerDef = hediffMakerProps.GetHediffMakerDef(comp, this, targetBodyPart);
        HediffDef hediffDef = hediffMakerDef.HediffDef;
        if (!comp.Pawn.health.hediffSet.TryGetHediff(hediffDef, out Hediff? existingHediff))
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
        else
        {
            // otherwise, we update the existing hediff's severity
            existingHediff.Severity += hediffMakerDef.GetInitialSeverity();
        }
    }

    protected virtual void PostApplyHediff(HediffComp_SecondaryCondition comp, Hediff hediff)
    {
        // This method can be overridden to perform additional actions after the hediff is created
    }

    protected virtual Hediff MakeHediff(HediffComp_SecondaryCondition sourceComp, HediffDef hediffDef, BodyPartRecord targetBodyPart, float severity)
    {
        Hediff hediff = HediffMaker.MakeHediff(hediffDef, sourceComp.Pawn);
        hediff.Severity = severity;
        if (hediff is HediffWithComps hediffWithComps)
        {
            bool setCausedBy = false;
            hediffWithComps.comps ??= [];
            foreach (HediffComp? comp in hediffWithComps.comps)
            {
                if (comp is HediffComp_CausedBy compCausedBy)
                {
                    // set the CausedBy property to the reason this hediff was created
                    compCausedBy.CausedBy = sourceComp.parent.Label.Colorize(sourceComp.parent.LabelColor);
                    setCausedBy = true;
                    break;
                }
            }
            if (!setCausedBy)
            {
                // if no HediffComp_CausedBy was found, we add one
                hediffWithComps.comps.Add(new HediffComp_CausedBy { CausedBy = sourceComp.parent.Label });
            }
        }
        sourceComp.Pawn.health.AddHediff(hediff, targetBodyPart);
        return hediff;
    }
}
