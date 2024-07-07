using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries;

public class fixleg : JobDriver
{

    protected Pawn Patient
    {
        get
        {
            return (Pawn)job.GetTarget(TargetIndex.A).Thing;
        }
    }

    protected Pawn Doctor
    {
        get
        {
            return pawn;
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
        return Doctor.Reserve(Patient, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Toil imtired = Toils_Goto.Goto(TargetIndex.A, PathEndMode.ClosestTouch);
        imtired.AddFinishAction(delegate { Patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp; });
        yield return imtired;

        float lvl = GetActor().health.capacities.GetLevel(PawnCapacityDefOf.Manipulation);

        if (lvl < 0.02f)
        {
            lvl = 0.01f;
        }

        Toil racialsur = Toils_General.Wait((int)Math.Round((480 / (GetActor().skills.GetSkill(SkillDefOf.Medicine).Level)) / lvl));
        racialsur.AddFinishAction(delegate
        {
            Hediff hedf = HediffMaker.MakeHediff(somedefof.FractureHealing, Patient, Patient.health.hediffSet.hediffs.Find(tt32 => tt32.def == somedefof.Fracture).Part);
            hedf.Severity = 1f;
            Patient.health.AddHediff(hedf);
            Patient.health.hediffSet.hediffs.RemoveAll(t => t.def == DefDatabase<HediffDef>.AllDefs.ToList().Find(iwnanadie => iwnanadie.defName == "Fracture"));

        });
        yield return racialsur;
    }

    private const TargetIndex targetInd = TargetIndex.A;
}
