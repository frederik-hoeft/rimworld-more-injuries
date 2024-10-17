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
        List<Hediff> bionics = pawn.health.hediffSet.hediffs.FindAll(hediff => hediff.def.addedPartProps is not null && hediff.def.HasModExtension<FixMisplacedBionicsModExtension>());

        foreach (Hediff bionic in bionics)
        {
            FixMisplacedBionicsModExtension bionicProperties = bionic.def.GetModExtension<FixMisplacedBionicsModExtension>();

            List<BodyPartRecord> bodyParts =
            [
                .. pawn.health.hediffSet.GetNotMissingParts().Where(part => bionicProperties.TargetedBodyPartsByRecipe.Contains(part.def))
            ];

            if (bodyParts.Count > 0)
            {
                bionic.Part = bodyParts.SelectRandom();
            }
            else
            {
                pawn.health.RemoveHediff(bionic);
            }
        }
    }
}
