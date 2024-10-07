using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

public class Recipe_BloodTransfusion_Fallback : Recipe_Surgery
{
    public override bool CompletableEver(Pawn surgeryTarget) => 
        surgeryTarget.health.hediffSet.HasHediff(HediffDefOf.BloodLoss) 
        && base.CompletableEver(surgeryTarget);

    public override bool AvailableOnNow(Thing thing, BodyPartRecord? part = null)
    {
        if (thing.MapHeld is null)
        {
            return false;
        }
        bool hasMedicine = false;
        foreach (Thing anyThing in thing.MapHeld.listerThings.AllThings)
        {
            if (anyThing.def == ThingDefOf.MedicineHerbal || anyThing.def == ThingDefOf.MedicineIndustrial || anyThing.def == ThingDefOf.MedicineUltratech)
            {
                hasMedicine = true;
                break;
            }
        }
        return hasMedicine && (thing is not Pawn pawn || pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss)) && base.AvailableOnNow(thing, part);
    }

    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        float offset = 0f;
        if (ingredients.Count > 0 && ingredients[0] is Thing medicine)
        {
            offset = medicine switch
            {
                not null when medicine.def == ThingDefOf.MedicineHerbal => 0.2f,
                not null when medicine.def == ThingDefOf.MedicineIndustrial => 0.35f,
                not null when medicine.def == ThingDefOf.MedicineUltratech => 1f,
                _ => 0f
            };
        }
        if (offset > 0.0)
        {
            Hediff bloodLoss = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLoss is not null)
            {
                bloodLoss.Severity -= offset;
            }
        }
        for (int index = 0; index < ingredients.Count; ++index)
        {
            if (!ingredients[index].Destroyed)
            {
                ingredients[index].Destroy();
            }
        }
    }

    public override float GetIngredientCount(IngredientCount ing, Bill bill)
    {
        if (bill.billStack?.billGiver is not Pawn billGiver || billGiver.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss) is null)
        {
            return base.GetIngredientCount(ing, bill);
        }
        return 2f;
    }
}