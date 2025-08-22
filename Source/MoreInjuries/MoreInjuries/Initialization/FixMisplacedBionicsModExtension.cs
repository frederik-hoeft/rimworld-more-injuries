using MoreInjuries.Extensions;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.Initialization;

public class FixMisplacedBionicsModExtension : DefModExtension
{
    public required List<BodyPartDef> TargetedBodyPartsByRecipe { get; set; }

    public static void FixPawn(Pawn pawn)
    {
        List<Hediff> bionics = pawn.health.hediffSet.hediffs.FindAll(static hediff => hediff.def.addedPartProps is not null && hediff.def.HasModExtension<FixMisplacedBionicsModExtension>());

        foreach (Hediff bionic in bionics)
        {
            FixMisplacedBionicsModExtension bionicProperties = bionic.def.GetModExtension<FixMisplacedBionicsModExtension>();
            if (bionic.Part?.def is null || bionicProperties.TargetedBodyPartsByRecipe.Contains(bionic.Part.def))
            {
                // this is fine. the bionic is on an allowed part
                continue;
            }

            List<BodyPartRecord> bodyParts =
            [
                .. pawn.health.hediffSet.GetNotMissingParts().Where(part => bionicProperties.TargetedBodyPartsByRecipe.Contains(part.def))
            ];

            if (bodyParts.Count > 0)
            {
                bionic.Part = bodyParts.SelectRandomOrDefault();
            }
            else
            {
                pawn.health.RemoveHediff(bionic);
            }
        }
    }
}
