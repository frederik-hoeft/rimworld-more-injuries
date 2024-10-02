using System.Collections.Generic;
using System.Linq;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

public class HearingLossComp : ThingComp
{
    public float GetHearingDamageMultiplier(Pawn pawn)
    {
        float result = 1f;
        // check if anything is worn that covers the ears
        if (pawn.apparel is { WornApparel.Count: > 0 })
        {
            // first, we need to get the ears
            IEnumerable<BodyPartRecord> ears = pawn.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def == KnownBodyPartDefOf.Ear);
            // does it cover the ears?
            // TODO: I don't think we need the specific ear here, just using the defs and building up a cache of covered body parts should be enough
            if (ears.FirstOrDefault() is BodyPartRecord ear && pawn.apparel.WornApparel.Any(clothing => clothing.def.apparel.CoversBodyPart(ear)))
            {
                result /= 5f;
            }
        }

        if (IsIndoors(pawn))
        {
            // loud noises are more damaging indoors (e.g. gunshots)
            result *= 3;
        }
        return result;
    }

    public override void Notify_UsedWeapon(Pawn pawn)
    {
        // early exit if the pawn is not a human
        if (pawn.def != ThingDefOf.Human)
        {
            return;
        }
        // early exit if the pawn is not equipped with a gun
        if (pawn.equipment?.Primary?.TryGetComp<CompEquippable>()?.PrimaryVerb.verbProps is not VerbProperties { range: > 0 } gunProperties)
        {
            return;
        }
        float radius = gunProperties.muzzleFlashScale;
        // when shot indoors, the gunshot sound is damaging over a larger area
        if (IsIndoors(pawn))
        {
            radius *= 1.25f;
        }
        // get all pawns in the vicinity of the shooter and apply hearing damage
        // TODO FIXME: may attempt to get cells outside of the map :(
        IEnumerable<IntVec3> cellsInVicinity = GenRadial.RadialCellsAround(pawn.Position, radius, useCenter: true);
        foreach (IntVec3 cell in cellsInVicinity)
        {
            List<Thing> pawnsInCell = cell.GetThingList(pawn.Map);
            for (int i = 0; i < pawnsInCell.Count; i++)
            {
                if (pawnsInCell[i] is Pawn otherPawn)
                {
                    float hearingDamageMultiplier = GetHearingDamageMultiplier(otherPawn);
                    if (Rand.Chance(hearingDamageMultiplier / 10f))
                    {
                        if (!otherPawn.health.hediffSet.TryGetHediff(KnownHediffDefOf.HearingLoss, out Hediff? hearingLoss))
                        {
                            hearingLoss = HediffMaker.MakeHediff(KnownHediffDefOf.HearingLoss, otherPawn);
                            otherPawn.health.AddHediff(hearingLoss);
                        }
                        hearingLoss.Severity += hearingDamageMultiplier / 100f;
                    }
                }
            }
        }
    }

    private static bool IsIndoors(Pawn pawn) => !pawn.Position.UsesOutdoorTemperature(pawn.Map);
}