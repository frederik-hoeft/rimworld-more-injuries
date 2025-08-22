using MoreInjuries.Caching;
using System.Runtime.CompilerServices;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

public sealed class HediffCompHandler_SecondaryCondition_Tick_SingleExecutionPerInterval : HediffCompHandler_SecondaryCondition_Tick
{
    private readonly ConditionalWeakTable<Pawn, TimedDataEntry<bool>> _perHediffDefSingletonCache = [];

    public override bool ShouldSkip(HediffComp_SecondaryCondition comp)
    {
        if (base.ShouldSkip(comp))
        {
            return true;
        }
        // check if we already executed this handler for this pawn in this tick interval
        int ticks = Find.TickManager.TicksGame;
        if (_perHediffDefSingletonCache.TryGetValue(comp.Pawn, out TimedDataEntry<bool> entry))
        {
            if (entry.TimeStamp + TickInterval > ticks)
            {
                // we already executed this handler for this pawn in this tick interval
                return true;
            }
            // did not execute in this tick interval, so we can update the timestamp
            entry.Initialize(true, ticks);
        }
        else
        {
            // first time we execute this handler for this pawn, so we add it to the cache
            entry = new TimedDataEntry<bool>();
            entry.Initialize(true, ticks);
            _perHediffDefSingletonCache.AddOrUpdate(comp.Pawn, entry);
            Logger.LogDebug($"Initialized new entry for {comp.Pawn} in {nameof(HediffCompHandler_SecondaryCondition_Tick_SingleExecutionPerInterval)} for {comp.parent.def.defName}");
        }
        return false;
    }
}