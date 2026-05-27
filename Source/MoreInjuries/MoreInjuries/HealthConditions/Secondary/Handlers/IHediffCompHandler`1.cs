using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

public interface IHediffCompHandler<in T> : IHediffCompHandler where T : HediffComp
{
    void Handle(T comp);
}
