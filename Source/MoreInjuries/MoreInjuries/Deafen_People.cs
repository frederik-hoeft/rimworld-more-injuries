using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoreInjuries;

[DefOf]
public class HearDmg : DefOf
{
    public static HediffDef HearingDamage;
}

public class DeafComp : ThingComp
{
    public float CalcHearingDamageMult(Pawn pawn)
    {
        float result = 1f;
        if (pawn.apparel != null)
        {
            List<BodyPartRecord> ear = pawn.health.hediffSet.GetNotMissingParts().ToList().FindAll(ttt => ttt.def.defName == "Ear");

            if (ear.Count > 0)
            {
                if (pawn != null && pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Count > 0)
                {
                    if (pawn.apparel.WornApparel.Any(test => test.def.apparel.CoversBodyPart(ear.First())))
                    {
                        result /= 5;
                    }
                }
            }
        }

        if (!pawn.Position.UsesOutdoorTemperature(pawn.Map))
        {
            result *= 3;
        }
        return result;
    }

    public override void Notify_UsedWeapon(Pawn pawn)
    {
        if (pawn.def == ThingDefOf.Human)
        {
            bool hm = (pawn.equipment?.Primary?.TryGetComp<CompEquippable>()?.PrimaryVerb.verbProps?.range ?? 0) > 0;
            if (hm)
            {
                float maybe = pawn.equipment?.Primary?.TryGetComp<CompEquippable>()?.PrimaryVerb.verbProps?.muzzleFlashScale ?? 0;

                if (pawn.Position.IsInside(pawn))
                {
                    maybe *= 1.25f;
                }

                //
                IEnumerable<IntVec3> varA = GenRadial.RadialCellsAround(pawn.Position, maybe, true);
                List<Pawn> varB = new();

                if (varA.Count() > 0)
                {
                    foreach (IntVec3 vec in varA)
                    {
                        if (vec.GetFirstPawn(pawn.Map) != null)
                        {
                            varB.Add(vec.GetFirstPawn(pawn.Map));
                        }
                    }
                }
                if (varB.Count > 0)
                {
                    foreach (Pawn pwann in varB)
                    {
                        if (pwann != null)
                        {
                            //
                            if (Rand.Chance((CalcHearingDamageMult(pwann) / 10f)))
                            {
                                //
                                if (pwann.health.hediffSet.HasHediff(HearDmg.HearingDamage))
                                {
                                    //
                                    Hediff varG = pwann.health.hediffSet.hediffs.Find(penis => penis.def == HearDmg.HearingDamage);
                                    //
                                    varG.Severity += (CalcHearingDamageMult(pwann) / 100f);
                                    //

                                }
                                else
                                {
                                    //
                                    Hediff varC = HediffMaker.MakeHediff(HearDmg.HearingDamage, pwann);
                                    //
                                    varC.Severity = CalcHearingDamageMult(pwann) / 100f;
                                    //
                                    pwann.health.AddHediff(varC);
                                    //
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

