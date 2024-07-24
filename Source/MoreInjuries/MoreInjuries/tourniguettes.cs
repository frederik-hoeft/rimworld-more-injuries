using System.Collections.Generic;
using System.Linq;
using MoreInjuries.HealthConditions.HearingLoss;
using MoreInjuries.Hemostat;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries;

[DefOf]
public class A_JobDefOf : DefOf
{
    public static JobDef tourni;

    public static HediffDef tournihed;

    public static ThingDef tourniquet;

    public static HediffDef ChokingOnTourni;

    public static BodyPartDef Skull;
}

[StaticConstructorOnStartup]
public class FuckPatching
{
    static FuckPatching()
    {
        A_JobDefOf.Skull.hitPoints = 35;

        //BodyPartDefOf.Brain.hitPoints = 10;

        BodyPartDefOf.Head.hitPoints = 20;

        //BodyPartDefOf.Jaw.hitPoints = 20;

        ThingDefOf.Human.comps.Add(new CompProperties { compClass = typeof(TourniComp) });

        ThingDefOf.Human.comps.Add(new CompProperties { compClass = typeof(HemostatComp) });

        if (MoreInjuriesMod.Settings.HearDMG)
        {
            IEnumerable<ThingDef> guns = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.Verbs?.Any(t2 => t2.range > 0) is true);
            foreach (ThingDef def in guns)
            {
                def.comps ??= [];
                def.comps.Add(new CompProperties { compClass = typeof(HearingLossComp) });
            }
        }

        MoreInjuriesMod.Settings.fuckYourFun = false;

        JobDefOf.TendPatient.driverClass = typeof(JobDriver_TendPatient);
    }
}

public class TourniComp : ThingComp
{
    public BodyPartRecord taggedBodyPart;

    public List<Hediff_Injury> taggedInjuries;

    public Hediff_MissingPart missingPart;

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        Pawn dad = (Pawn)parent;
        if (dad.Downed)
        {
            if (dad.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) > 0.19f)
            {
                if (dad.CellsAdjacent8WayAndInside().Any(ttt => ttt.GetFirstThing(dad.Map, A_JobDefOf.tourniquet) != null) | dad.inventory.innerContainer.Any(x => x.def == A_JobDefOf.tourniquet))
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Apply a tourniquet",
                        defaultDesc = "While downed, the pawn still can attempt applying a tourniquet to themselves",
                        icon = ContentFinder<Texture2D>.Get("UI/tour_gizmo", true),
                        action = delegate
                        {
                            List<FloatMenuOption> options = new();

                            Pawn parent = this.parent as Pawn;
                            List<BodyPartRecord> Limbs = parent.health.hediffSet.GetNotMissingParts().ToList().FindAll(tpt => tpt.def.defName == "Shoulder" | tpt.def.defName == "Leg");
                            foreach (BodyPartRecord bodypart in Limbs)
                            {

                                if (true)
                                {
                                    Thing tourni = dad.CellsAdjacent8WayAndInside().ToList().FindAll(pensi => pensi.GetFirstThing(dad.Map, A_JobDefOf.tourniquet) != null).RandomElement().GetFirstThing(dad.Map, A_JobDefOf.tourniquet);
                                    options.Add(new FloatMenuOption("Apply a tourniquet to " + bodypart.Label.Colorize(Color.green), delegate
                                    {

                                        taggedBodyPart = bodypart;

                                        taggedInjuries = parent.health.hediffSet.GetHediffsTendable()
                                            .OfType<Hediff_Injury>()
                                            .Where(ttt => (ttt.Bleeding) 
                                                && (ttt.Part == taggedBodyPart 
                                                    | (bodypart.parts?.Contains(ttt.Part) ?? false) 
                                                    | (bodypart.parts?.Any(ppp => ppp.parts?.Contains(ttt.Part) ?? false) ?? false)))
                                            .ToList();

                                        BodyPartRecord varA = parent.TryGetComp<TourniComp>().taggedBodyPart;
                                        Hediff varB = HediffMaker.MakeHediff(A_JobDefOf.tournihed, parent, varA);
                                        List<Hediff_Injury> varC = parent.TryGetComp<TourniComp>().taggedInjuries;
                                        if (varC != null)
                                        {
                                            foreach (Hediff_Injury injur in varC)
                                            {
                                                if (injur != null)
                                                {

                                                    if (injur is BetterInjury)
                                                    {
                                                        ((BetterInjury)injur).IsBase = false;
                                                        ((BetterInjury)injur).OverriddenBleedRate = 0.01f;
                                                    }
                                                }
                                            }
                                        }
                                        varB.TryGetComp<TourniHediffComp>().injuries = varC;
                                        varB.Severity = 1.1f;
                                        parent.health.AddHediff(varB, varA);
                                        if (tourni.stackCount > 0)
                                        {
                                            tourni.stackCount--;
                                        }
                                        else
                                        {
                                            tourni.Destroy();
                                        }

                                        if (tourni.stackCount == 0 && !tourni.Destroyed)
                                        {
                                            tourni.Destroy();
                                        }
                                    }));
                                }
                                Find.WindowStack.Add(new FloatMenu(options));
                            }
                        }
                    };

                }
            }
        }
        else
        {
            if (dad.inventory.innerContainer.Any(x => x.def == A_JobDefOf.tourniquet))
            {
                yield return new Command_Action
                {
                    defaultLabel = "Apply a tourniquet",
                    defaultDesc = "Apply a tourniquet to themselves",
                    icon = ContentFinder<Texture2D>.Get("UI/tour_gizmo", true),
                    action = delegate
                    {
                        List<FloatMenuOption> options = new();

                        Pawn parent = this.parent as Pawn;
                        List<BodyPartRecord> Limbs = parent.health.hediffSet.GetNotMissingParts().ToList().FindAll(tpt => tpt.def.defName == "Shoulder" | tpt.def.defName == "Leg");
                        foreach (BodyPartRecord bodypart in Limbs)
                        {
                            if (true)
                            {
                                //var tourni = dad.CellsAdjacent8WayAndInside().ToList().FindAll(pensi => pensi.GetFirstThing(dad.Map, A_JobDefOf.tourniquet) != null).RandomElement().GetFirstThing(dad.Map, A_JobDefOf.tourniquet);
                                options.Add(new FloatMenuOption("Apply a tourniquet to " + bodypart.Label.Colorize(Color.green), delegate
                                {
                                    //missingPart = (Hediff_MissingPart)parent.health.hediffSet.hediffs.Find(x => x.def.hediffClass == typeof(Hediff_MissingPart) && (!x.Bleeding | !x.IsTended()));

                                    taggedBodyPart = bodypart;

                                    taggedInjuries = parent.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>().ToList().FindAll(ttt => (ttt.Bleeding) && (ttt.Part == taggedBodyPart | (bodypart.parts?.Contains(ttt.Part) ?? false) | (bodypart.parts?.Any(ppp => ppp.parts?.Contains(ttt.Part) ?? false) ?? false)));

                                    Job varA = new() { def = A_JobDefOf.tourni, targetA = this.parent, targetB = dad.inventory.innerContainer.ToList().Find(K => K.def.defName == "tourniquet") };
                                    dad.jobs.StartJob(varA, JobCondition.InterruptForced);

                                }));
                            }
                        }
                        Find.WindowStack.Add(new FloatMenu(options));
                    }
                };
            }
        }
    }

    public bool BodyPartHasHediffOfDef(HediffDef hediff, BodyPartRecord bdpart, Pawn pawn)
    {
        bool result = false;

        if (!pawn.health.hediffSet.hediffs.FindAll(tptt => tptt.Part == bdpart).Any(ppptt => ppptt.def == hediff))
        {
            result = true;
        }

        return result;
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        Pawn parent = this.parent as Pawn;

        bool test = true;

        if (parent.Faction.HostileTo(Faction.OfPlayer) && !parent.Downed)
        {
            test = false;
        }

        if (test)
        {
            List<BodyPartRecord> Limbs = parent.health.hediffSet.GetNotMissingParts().ToList().FindAll(tpt => tpt.def.defName == "Shoulder" | tpt.def.defName == "Leg");

            bool dumbass = ((selPawn.story.traits.allTraits.Any(fgp => fgp.def.defName == "SlowLearner")) && (selPawn.skills.skills.Find(ggp => ggp.def == SkillDefOf.Medicine).Level < 5));

            bool med_dumbass = selPawn.skills.skills.Find(ggp => ggp.def == SkillDefOf.Medicine).Level == 0;

            if (dumbass | med_dumbass)
            {
                BodyPartRecord neck = parent.health.hediffSet.GetNotMissingParts().ToList().Find(shite => shite.def.defName == "Neck");
                Limbs.Add(neck);
            }

            if (selPawn.inventory.innerContainer.Any(F => F.def.defName == "tourniquet"))
            {
                foreach (BodyPartRecord bodypart in Limbs)
                {
                    //if (BodyPartHasHediffOfDef(A_JobDefOf.tournihed, bodypart, parent))
                    if (true)
                    {
                        yield return new FloatMenuOption("Apply a tourniquet to " + bodypart.Label.Colorize(Color.green), delegate
                        {
                            //missingPart = (Hediff_MissingPart)parent.health.hediffSet.hediffs.Find(x => x.def.hediffClass == typeof(Hediff_MissingPart) && (!x.Bleeding | !x.IsTended()));

                            taggedBodyPart = bodypart;

                            taggedInjuries = parent.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>().ToList().FindAll(ttt => (ttt.Bleeding) && (ttt.Part == taggedBodyPart | (bodypart.parts?.Contains(ttt.Part) ?? false) | ttt.Part == bodypart | ((!bodypart.parent?.IsCorePart ?? false) && (bodypart.parent?.parts?.Contains(ttt.Part) ?? false)) | (bodypart.parts?.Any(ppp => ppp.parts?.Contains(ttt.Part) ?? false) ?? false)));

                            Job varA = new() { def = A_JobDefOf.tourni, targetA = this.parent, targetB = selPawn.inventory.innerContainer.ToList().Find(K => K.def.defName == "tourniquet") };
                            selPawn.jobs.StartJob(varA, JobCondition.InterruptForced);

                        });
                    }
                }
            }

            if (parent.health.hediffSet.hediffs.FindAll(hed => hed.def == A_JobDefOf.tournihed).Count > 0)
            {
                foreach (Hediff hedif in parent.health.hediffSet.hediffs.FindAll(hed => hed.def == A_JobDefOf.tournihed))
                {
                    yield return new FloatMenuOption("Take off the tourniquet on: " + hedif.Part.Label.Colorize(new Color(130, 0, 130)), delegate
                    {
                        parent.health.RemoveHediff(hedif);
                    });
                }
            }
        }
    }
}

public class PutOnTheThing : JobDriver
{

    public Pawn Patient
    {
        get
        {
            return (Pawn)job.GetTarget(TargetIndex.A).Thing;
        }
    }

    public Pawn Doctor
    {
        get
        {
            return pawn;
        }
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return Doctor.Reserve(Patient, job, 1, 1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);

        Toil toil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return toil;
        Patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
        Toil toil2 = Toils_General.Wait(60);
        toil2.AddFinishAction(delegate
        {
            BodyPartRecord varA = Patient.TryGetComp<TourniComp>().taggedBodyPart;
            Hediff varB = HediffMaker.MakeHediff(A_JobDefOf.tournihed, Patient, varA);
            List<Hediff_Injury>? varC = Patient.TryGetComp<TourniComp>().taggedInjuries;
            if (varC != null)
            {
                foreach (Hediff_Injury injur in varC)
                {
                    if (injur != null)
                    {
                        if (injur is BetterInjury)
                        {
                            ((BetterInjury)injur).IsBase = false;
                            ((BetterInjury)injur).OverriddenBleedRate = 0.01f;
                        }
                    }
                }
            }
            /*
            if (varA.def.defName == "Shoulder")
            {
                if (GetActor().health.hediffSet.hediffs.FindAll(x => x is BetterPartMissing).Find(x => (x.Part?.def ?? null) == BodyPartDefOf.Arm | x.sourceBodyPartGroup.defName.ToLower().Contains("arm")) != null)
                {
                    ((BetterPartMissing)GetActor().health.hediffSet.hediffs.FindAll(x => x is BetterPartMissing).Find(x => (x.Part?.def ?? null) == BodyPartDefOf.Arm | x.sourceBodyPartGroup.defName.ToLower().Contains("arm"))).BleedRateInt *= 0.1f;
                }
            }
            */
            varB.TryGetComp<TourniHediffComp>().injuries = varC;
            varB.Severity = 1.1f;
            Patient.health.AddHediff(varB, varA);
            if (TargetB.Thing.stackCount > 0)
            {
                --TargetB.Thing.stackCount;
            }
            else
            {
                TargetB.Thing.Destroy();
            }

            if (TargetB.Thing.stackCount == 0)
            {
                TargetB.Thing.Destroy();
            }

            //var missingParts = Patient.TryGetComp<TourniComp>().missingPart;

            //if (missingParts != null && missingParts.Part?.def.defName == "Arm")
            //{
            //if (Patient.TryGetComp<TourniComp>().taggedBodyPart.def.defName == "Shoulder" && !missingParts.IsTended())
            //{
            //missingParts.Tended(1f, 1f);
            //}

            //}

        });
        yield return toil2;
        yield break;
    }
}

public class TourniHediffComp : HediffComp
{
    public List<Hediff_Injury> injuries;

    public override void CompPostPostRemoved()
    {
        if (injuries != null)
        {
            foreach (Hediff_Injury injur in injuries)
            {
                if (injur != null)
                {
                    if (injur is BetterInjury)
                    {
                        ((BetterInjury)injur).IsBase = true;
                    }
                }
            }
        }
        base.CompPostPostRemoved();
    }

    public override void CompPostMake()
    {
        if (parent.Part.def.defName == "Neck")
        {
            Hediff hed = HediffMaker.MakeHediff(A_JobDefOf.ChokingOnTourni, parent.pawn);

            parent.pawn.health.AddHediff(hed);
        }

        base.CompPostMake();
    }
}

