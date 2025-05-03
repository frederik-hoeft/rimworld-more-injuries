using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.Initialization;

[StaticConstructorOnStartup]
public class FixMisplacedBionics_Initializer
{
    static FixMisplacedBionics_Initializer()
    {
        IEnumerable<RecipeDef> bionicsRecipeDefs = DefDatabase<RecipeDef>.AllDefs.Where(static def => def is 
        {
            // adds a non-natural body part
            addsHediff.addedPartProps: not null,
            appliedOnFixedBodyParts.Count: > 0 
        });

        foreach (RecipeDef recipeDef in bionicsRecipeDefs)
        {
            recipeDef.addsHediff.modExtensions ??= [];
            recipeDef.addsHediff.modExtensions.Add(new FixMisplacedBionicsModExtension
            {
                TargetedBodyPartsByRecipe = recipeDef.appliedOnFixedBodyParts
            });
        }
    }
}
