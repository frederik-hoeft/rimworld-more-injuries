using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffCompHandler_SecondaryCondition_TargetsBodyPart : HediffCompHandler_SecondaryCondition
{
    // don't rename this field. XML defs depend on this name
    private readonly BodyPartHediffTargetEvaluator targetEvaluator = default!;
    // don't rename this field. XML defs depend on this name
    protected readonly float minSeverity = 0f;
    // don't rename this field. XML defs depend on this name
    protected readonly float maxSeverity = 0f;
    // don't rename this field. XML defs depend on this name
    protected readonly bool allowDuplicate = false;
    // don't rename this field. XML defs depend on this name
    protected readonly bool allowMultiple = false;

    public override void Evaluate(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        if (ShouldSkip(comp, severityAdjustment))
        {
            return;
        }
        Logger.LogDebug($"Evaluating {comp.parent.LabelCap} ({HediffDef.defName}) for {comp.Pawn.Name} with severity {comp.parent.Severity}");
        if (targetEvaluator is null)
        {
            Logger.Error($"{nameof(HediffCompHandler_SecondaryCondition_TargetsBodyPart)}: {comp.GetType().Name} has no target evaluator defined. Cannot evaluate.");
            return;
        }
        BodyPartRecord? targetBodyPart = targetEvaluator.GetTargetBodyPart(comp, this);
        if (targetBodyPart is null)
        {
            Logger.LogDebug($"No valid target body part found for {comp.Pawn.Name} ({comp.parent.LabelCap}). Skipping evaluation.");
            return;
        }
        if (!comp.Pawn.health.hediffSet.TryGetHediff(HediffDef, out Hediff? existingHediff))
        {
            Hediff hediff = MakeHediff(comp.Pawn, targetBodyPart);
            PostApplyHediff(comp, hediff);
            return;
        }
        if (!allowMultiple)
        {
            // if the hediff already exists, we don't need to create a new one
            Logger.LogDebug($"Hediff {HediffDef.defName} already exists for {comp.Pawn.Name} ({comp.parent.LabelCap}). Skipping creation.");
            return;
        }
        if (allowDuplicate)
        {
            // if duplicates are allowed, we can create a new hediff even if it already exists
            Hediff hediff = MakeHediff(comp.Pawn, targetBodyPart);
            PostApplyHediff(comp, hediff);
        }
        else
        {
            // otherwise, we update the existing hediff's severity
            existingHediff.Severity += GetInitialSeverity();
        }
    }

    protected virtual void PostApplyHediff(HediffComp_SecondaryCondition comp, Hediff hediff)
    {
        // This method can be overridden to perform additional actions after the hediff is created
    }

    protected virtual Hediff MakeHediff(Pawn pawn, BodyPartRecord targetBodyPart)
    {
        Hediff hediff = HediffMaker.MakeHediff(HediffDef, pawn);
        hediff.Severity = GetInitialSeverity();
        pawn.health.AddHediff(hediff, targetBodyPart);
        return hediff;
    }

    protected virtual float GetInitialSeverity()
    {
        if (minSeverity == maxSeverity)
        {
            return minSeverity;
        }
        if (maxSeverity == 0f)
        {
            return minSeverity;
        }
        if (minSeverity > maxSeverity)
        {
            Logger.Error($"{nameof(HediffCompHandler_SecondaryCondition_TargetsBodyPart)}: {nameof(minSeverity)} ({minSeverity}) is greater than {nameof(maxSeverity)} ({maxSeverity}). Using {nameof(minSeverity)} instead.");
            return minSeverity;
        }
        return Rand.Range(minSeverity, maxSeverity);
    }
}
