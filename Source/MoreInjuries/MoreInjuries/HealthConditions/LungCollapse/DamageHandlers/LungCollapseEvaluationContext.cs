using System.Runtime.InteropServices;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse.DamageHandlers;

[StructLayout(LayoutKind.Sequential)]
internal ref struct LungCollapseEvaluationContext
{
    public const int LUNG_COUNT = 2;
    // DO NOT TOUCH THIS CODE. YOU WILL BREAK MEMORY SAFETY
    public float ChanceLung0;
    public float ChanceLung1;
    public BodyPartRecord? Lung0;
    public BodyPartRecord? Lung1;

    public Span<float> ChancesPerLung => MemoryMarshal.CreateSpan(ref ChanceLung0, LUNG_COUNT);

    public Span<BodyPartRecord?> Lungs => MemoryMarshal.CreateSpan(ref Lung0, LUNG_COUNT);
}