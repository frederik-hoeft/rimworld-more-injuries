using MoreInjuries.AI;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.HealthConditions.HeavyBleeding;
using MoreInjuries.HealthConditions.HeavyBleeding.Bandages;
using MoreInjuries.HealthConditions.HeavyBleeding.HemostaticAgents;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions;

public class ProvideFirstAidWorker(MoreInjuryComp parent) : InjuryWorker(parent), ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = Target;
        if (patient != selectedPawn && selectedPawn.Drafted && !builder.Keys.Contains(UITreatmentOption.ProvideFirstAid))
        {
            builder.Keys.Add(UITreatmentOption.ProvideFirstAid);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, string.Empty) is not null)
            {
                return;
            }
            // check if we can treat any of the known conditions
            foreach (Hediff hediff in patient.health.hediffSet.hediffs)
            {
                if (JobDriver_HemostasisBase.JobCanTreat(hediff) 
                    || JobDriver_UseDefibrillator.JobCanTreat(hediff) 
                    || Array.IndexOf(JobDriver_UseSuctionDevice.TargetHediffDefs, hediff.def) != -1 
                    || Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1 
                    || patient.Downed && hediff.TendableNow())
                {
                    builder.Options.Add(new FloatMenuOption("Provide first aid", new JobDescriptor(selectedPawn, patient).StartJob));
                    return;
                }
            }
        }
    }

    private class JobDescriptor(Pawn doctor, Pawn patient) : IJobDescriptor
    {
        public Job CreateJob() => throw new NotSupportedException();

        public void StartJob()
        {
            JobQueueBuilder jobQueueBuilder = new(doctor);
            Job job;
            // first stop the bleeding (inventory first)
            if (patient.health.hediffSet.hediffs.Any(JobDriver_HemostasisBase.JobCanTreat))
            {
                if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryHemostaticAgent)
                {
                    job = JobDriver_UseHemostaticAgent.GetDispatcher(doctor, patient, inventoryHemostaticAgent, fromInventoryOnly: true).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                }
                if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryBandage)
                {
                    job = JobDriver_UseBandage.GetDispatcher(doctor, patient, inventoryBandage, fromInventoryOnly: true).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                }
                if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat) is Thing hemostaticAgent)
                {
                    job = JobDriver_UseHemostaticAgent.GetDispatcher(doctor, patient, hemostaticAgent, fromInventoryOnly: false).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                }
                if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat) is Thing bandage)
                {
                    job = JobDriver_UseBandage.GetDispatcher(doctor, patient, bandage, fromInventoryOnly: false).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                }
            }
            // next, see if we need to defibrillate
            bool performingCpr = false;
            if (patient.health.hediffSet.hediffs.Any(JobDriver_UseDefibrillator.JobCanTreat))
            {
                if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Defibrillator, JobDriver_UseDefibrillator.JobCanTreat, fromInventoryOnly: true) is Thing defibrillator)
                {
                    job = JobDriver_UseDefibrillator.GetDispatcher(doctor, patient, defibrillator).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                }
                else
                {
                    job = JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                    performingCpr = true;
                }
            }
            // is the patient choking?
            if (!performingCpr && patient.health.hediffSet.hediffs.Any(hediff => Array.IndexOf(JobDriver_UseSuctionDevice.TargetHediffDefs, hediff.def) != -1))
            {
                if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.SuctionDevice, JobDriver_UseSuctionDevice.TargetHediffDefs, fromInventoryOnly: true) is Thing suctionDevice)
                {
                    job = JobDriver_UseSuctionDevice.GetDispatcher(doctor, patient, suctionDevice).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                }
                else
                {
                    job = JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
                    jobQueueBuilder.StartOrSchedule(job);
                    performingCpr = true;
                }
            }
            // do we need to perform CPR?
            if (!performingCpr && patient.health.hediffSet.hediffs.Any(hediff => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1))
            {
                job = JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
                jobQueueBuilder.StartOrSchedule(job);
            }
            // is an immedite blood transfusion required?
            if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, JobDriver_UseBloodBag.JobDeviceDef, hediff => JobDriver_UseBloodBag.JobCanTreat(hediff, bloodLossThreshold: 0.65f), fromInventoryOnly: true) is Thing bloodBag)
            {
                job = JobDriver_UseBloodBag.GetDispatcher(doctor, patient, bloodBag, fromInventoryOnly: true, fullyHeal: false).CreateJob();
                jobQueueBuilder.StartOrSchedule(job);
            }
            // start normal vanilla treatment (only if the patient is downed because otherwise the patient will just get up and walk away)
            if (patient.Downed && patient.health.hediffSet.hediffs.Any(hediff => hediff.TendableNow()))
            {
                Thing medicine = HealthAIUtility.FindBestMedicine(doctor, patient, onlyUseInventory: true);
                job = JobMaker.MakeJob(JobDefOf.TendPatient, patient);
                job.count = 1;
                jobQueueBuilder.StartOrSchedule(job);
            }
        }
    }
}