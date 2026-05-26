namespace MoreInjuries.HealthConditions.Choking.Simulation;

internal readonly record struct NextChokingSimulationState(float Severity, float FluidBurden)
{
    private const float EPSILON = 0.001f;

    public bool IsResolved => Severity <= EPSILON && FluidBurden <= EPSILON;
}
