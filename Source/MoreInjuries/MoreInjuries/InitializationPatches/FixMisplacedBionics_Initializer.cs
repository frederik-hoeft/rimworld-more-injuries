using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.InitializationPatches;

[StaticConstructorOnStartup]
public class FixMisplacedBionics_Initializer
{
    static FixMisplacedBionics_Initializer()
    {
        IEnumerable<RecipeDef> recipesAppliedToFixedBodyParts = DefDatabase<RecipeDef>.AllDefs.Where(def =>
            def is { addsHediff.addedPartProps: not null, appliedOnFixedBodyParts.Count: > 0 });

        foreach (RecipeDef recipeDef in recipesAppliedToFixedBodyParts)
        {
            recipeDef.addsHediff.modExtensions ??= [];
            recipeDef.addsHediff.modExtensions.Add(new FixMisplacedBionicsModExtension
            {
                BodyParts = recipeDef.appliedOnFixedBodyParts
            });
        }
    }
}
