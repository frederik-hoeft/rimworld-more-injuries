using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffCompHandler_LinkedSeverity : HediffCompHandler
{
    // don't rename this field. XML defs depend on this name
    private readonly List<SecondaryHediffModifier>? severityModifiers = default;
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef? linkedHediffDef = default;

    public sealed override int TickInterval => throw new NotSupportedException($"{nameof(HediffCompHandler_LinkedSeverity)} does not support {nameof(TickInterval)}.");

    public HediffDef LinkedHediffDef => linkedHediffDef ?? throw new InvalidOperationException($"{nameof(HediffCompHandler_LinkedSeverity)}: {nameof(linkedHediffDef)} is not defined. Cannot evaluate.");

    public virtual float Evaluate(Hediff hediff)
    {
        float severity = hediff.Severity;
        if (severity < Mathf.Epsilon)
        {
            return 0f;
        }
        if (severityModifiers is not { Count: > 0 })
        {
            return severity;
        }
        foreach (SecondaryHediffModifier modifier in severityModifiers)
        {
            severity *= modifier.GetModifier(hediff, this);
            if (severity <= Mathf.Epsilon)
            {
                Logger.LogDebug($"Severity for {hediff.pawn.Name} ({hediff.LabelCap}) to get the secondary condition is 0. Skipping evaluation.");
                return 0f;
            }
        }
        return severity;
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