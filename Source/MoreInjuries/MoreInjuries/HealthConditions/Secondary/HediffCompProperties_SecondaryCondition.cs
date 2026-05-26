using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary;

[XmlBindable]
public partial class HediffCompProperties_SecondaryCondition : HediffCompProperties<HediffComp_SecondaryCondition>
{
    private static readonly List<IHediffCompHandler<HediffComp_SecondaryCondition>> s_emptyHandlers = [];

    [XmlBinding("severityCurve")]
    public partial SimpleCurve? SeverityCurve { get; }

    [XmlBinding("handlers", DefaultValueFrom = nameof(s_emptyHandlers))]
    public partial IReadOnlyList<IHediffCompHandler<HediffComp_SecondaryCondition>> Handlers { get; }

    public IReadOnlyList<IHediffComp_SecondaryCondition_TickHandler> TickHandlers => field ??=
    [
        .. Handlers.OfType<IHediffComp_SecondaryCondition_TickHandler>()
    ];

    public IReadOnlyList<IHediffComp_SecondaryCondition_PostMakeHandler> PostMakeHandlers => field ??=
    [
        .. Handlers.OfType<IHediffComp_SecondaryCondition_PostMakeHandler>()
    ];
}
