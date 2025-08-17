using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse.DamageHandlers;

internal interface ILungCollapseHandler
{
    bool EvaluateDamageChances(LungCollapseWorker worker, ref readonly DamageInfo dinfo, ref LungCollapseEvaluationContext context, ref List<Hediff>? causedBy);
}