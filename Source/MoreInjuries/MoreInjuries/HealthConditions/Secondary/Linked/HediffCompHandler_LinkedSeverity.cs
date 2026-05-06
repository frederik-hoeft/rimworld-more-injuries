using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

[XmlSerializable]
public partial class HediffCompHandler_LinkedSeverity : HediffCompHandler
{
    [XmlMember("linkedHediffDef")]
    public partial HediffDef LinkedHediffDef { get; }

    [XmlMember("severityModifiers")]
    public partial IReadOnlyList<SecondaryHediffModifier>? SeverityModifiers { get; }

    public virtual float Evaluate(Hediff hediff)
    {
        float severity = hediff.Severity;
        if (severity < Mathf.Epsilon)
        {
            return 0f;
        }
        if (SeverityModifiers is not [_, ..] severityModifiers)
        {
            // no modifiers, just return the current severity of the hediff
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
