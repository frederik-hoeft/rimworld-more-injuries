using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries;

public class FixerModExt : DefModExtension
{
    public required List<BodyPartDef> BodyParts { get; set; }
}

[StaticConstructorOnStartup]
public class ConstructorFixPart
{
    static ConstructorFixPart()
    {
        IEnumerable<RecipeDef> recipesAppliedToFixedBodyParts = DefDatabase<RecipeDef>.AllDefs.Where(def => 
            def.addsHediff?.addedPartProps is not null
            && def.appliedOnFixedBodyParts is { Count: > 0 });

        foreach (RecipeDef recipeDef in recipesAppliedToFixedBodyParts)
        {
            recipeDef.addsHediff.modExtensions ??= [];
            recipeDef.addsHediff.modExtensions.Add(new FixerModExt 
            { 
                BodyParts = recipeDef.appliedOnFixedBodyParts
            });
        }
    }
}
