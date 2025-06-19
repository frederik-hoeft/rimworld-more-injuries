using MoreInjuries.BuildIntrinsics;
using MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;
using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class HediffCompHandler_SecondaryCondition
{
    // don't rename this field. XML defs depend on this name
    protected readonly float? baseChance = null;
    // don't rename this field. XML defs depend on this name
    private readonly int tickInterval = GenTicks.TickRareInterval;
    // don't rename this field. XML defs depend on this name
    private readonly HediffMakerProperties? hediffMakerProps = default;
    // don't rename this field. XML defs depend on this name
    private readonly List<SecondaryHediffChanceModifier>? chanceModifiers = default;

    public virtual float BaseChance => baseChance ?? 1f;

    public int TickInterval => tickInterval;

    public HediffMakerProperties? HediffMakerProps => hediffMakerProps;

    public virtual bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment)
    {
        if (!comp.Pawn.IsHashIntervalTick(TickInterval))
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
            foreach (SecondaryHediffChanceModifier modifier in chanceModifiers)
            {
                chance *= modifier.GetModifer(comp.Pawn);
                if (chance <= Mathf.Epsilon)
                {
                    Logger.LogDebug($"Chance for {comp.Pawn.Name} ({comp.parent.LabelCap}) to get the secondary condition is 0. Skipping evaluation.");
                    return true;
                }
            }
        }
        if (chance <= Mathf.Epsilon || !Rand.Chance(chance))
        {
            return true;
        }
        return false;
    }

    public abstract void Evaluate(HediffComp_SecondaryCondition comp, float severityAdjustment);
}