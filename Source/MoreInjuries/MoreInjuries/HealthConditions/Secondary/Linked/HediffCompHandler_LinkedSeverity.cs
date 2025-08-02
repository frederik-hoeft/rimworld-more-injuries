using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffCompHandler_LinkedSeverity : HediffCompHandler
{
    // don't rename this field. XML defs depend on this name
    private readonly List<SecondaryHediffModifier>? severityModifiers = default;
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef? linkedHediffDef = default;

    public HediffDef LinkedHediffDef
    {
        get
        {
            Throw.InvalidOperationException.IfNull(this, linkedHediffDef);
            return linkedHediffDef;
        }
    }

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
                Logger.LogDebug($"Secondary condition evaluation for {hediff.pawn.Name} ({hediff.Label}) failed. {modifier.GetType().Name} returned 0 severity or chance.");
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