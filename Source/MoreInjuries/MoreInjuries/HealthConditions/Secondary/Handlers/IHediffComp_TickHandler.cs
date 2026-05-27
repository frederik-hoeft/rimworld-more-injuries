namespace MoreInjuries.HealthConditions.Secondary.Handlers;

public interface IHediffComp_TickHandler : IHediffCompHandler
{
    int TickInterval { get; }
}
