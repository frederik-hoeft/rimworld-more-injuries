using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries;

public class DOCPR : JobDriver
{

    protected Pawn Patient
    {
        get
        {
            return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
        }
    }

    protected Pawn Doctor
    {
        get
        {
            return this.pawn;
        }
    }

    public JobCondition wtf(Pawn hurtdude)
    {
        if (!hurtdude.health.hediffSet.hediffs.Any(O => O.def == Caula_DefOf.ChokingOnBlood))
        {
            return JobCondition.Succeeded;
        }
        else
        {
            return JobCondition.Ongoing;
        }
    }

    //public 

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return this.Doctor.Reserve(this.Patient, this.job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnAggroMentalState(TargetIndex.A);

        Toil toil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        toil.AddFinishAction(delegate { Patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp; });
        yield return toil;
        Toil toil2 = Toils_General.Wait(60);
        Hediff heffid = Patient.health.hediffSet.GetFirstHediffOfDef(Caula_DefOf.ChokingOnBlood);
        toil2.AddFinishAction(delegate
        {

            if (heffid == null)
            {
                HediffDef smth = DefDatabase<HediffDef>.AllDefs.ToList().Find(O => O.defName == "HeartAttack");
                if (Patient.health.hediffSet.GetFirstHediffOfDef(smth) != null)
                {
                    heffid = Patient.health.hediffSet.GetFirstHediffOfDef(smth);
                }
            }
            //else if(heffid.def == Caula_DefOf.ChokingOnBlood)
            heffid.Severity -= ((Doctor.skills.skills.Find(o => o.def == SkillDefOf.Medicine).Level * 1.35f)) / 100;
        });
        Func<JobCondition> func = delegate
        {
            if (Patient.health.hediffSet.hediffs.Any(kurwa => kurwa.def == Caula_DefOf.ChokingOnBlood | kurwa.def.label == "heart attack"))
            {
                ////
                return JobCondition.Ongoing;
            }
            else
            {
                ////
                return JobCondition.Succeeded;
            }
        };

        //toil2.AddEndCondition(func);

        yield return toil2;

        this.AddEndCondition(func);

        //this.AddEndCondition()
    }

    private const TargetIndex targetInd = TargetIndex.A;
}
