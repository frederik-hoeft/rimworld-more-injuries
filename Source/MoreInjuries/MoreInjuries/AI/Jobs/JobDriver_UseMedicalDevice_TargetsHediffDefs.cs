using Verse;

namespace MoreInjuries.AI.Jobs;

public abstract class JobDriver_UseMedicalDevice_TargetsHediffDefs : JobDriver_UseMedicalDevice
{
    protected abstract HediffDef[] HediffDefs { get; }
    
    protected override bool IsTreatable(Hediff hediff) => Array.IndexOf(HediffDefs, hediff.def) != -1;
}
