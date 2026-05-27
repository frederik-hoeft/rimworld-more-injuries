using MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;

using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

[XmlBindable]
public abstract partial class HediffCompHandler_ChanceBased : IHediffCompHandler
{
    [XmlBinding<float>("baseChance", defaultValue: 1f)]
    public virtual partial float BaseChance { get; }

    [XmlBinding("chanceModifiers")]
    public partial IReadOnlyList<SecondaryHediffModifier>? ChanceModifiers { get; }

    protected virtual bool ShouldSkip(HediffComp_SecondaryCondition comp)
    {
        Hediff hediff = comp.parent;
        float chance = BaseChance;
        if (chance <= 0f)
        {
            return true;
        }
        if (ChanceModifiers is { Count: > 0 })
        {
            foreach (SecondaryHediffModifier modifier in ChanceModifiers)
            {
                chance *= modifier.GetModifier(hediff, compHandler: this);
                if (chance <= 0f)
                {
                    return true;
                }
            }
        }
        return !Rand.Chance(chance);
    }
}
