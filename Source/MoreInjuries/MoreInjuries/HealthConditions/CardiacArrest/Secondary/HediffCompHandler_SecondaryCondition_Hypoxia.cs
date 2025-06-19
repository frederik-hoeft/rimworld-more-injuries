using MoreInjuries.HealthConditions.Secondary.Handlers;

namespace MoreInjuries.HealthConditions.CardiacArrest.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_Hypoxia : HediffCompHandler_SecondaryCondition_TargetsBodyPart
{
    public override float BaseChance => MoreInjuriesMod.Settings.OrganHypoxiaChance * base.BaseChance;
}
