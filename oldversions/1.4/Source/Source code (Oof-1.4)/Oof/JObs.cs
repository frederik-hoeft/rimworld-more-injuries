using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using UnityEngine;

namespace Oof
{
    public class chokingcomp : HediffComp
    {
        public ToggleSettings Settings
        {
            get
            {
                return LoadedModManager.GetMod<OofMod>().GetSettings<ToggleSettings>();
            }
        }
        public int chocke_int;

        public Hediff_Injury source;

        public override void CompPostMake()
        {
            chocke_int = Props.ABCD;
            if (Settings.somesound)
            {
                this.Props.coughSound.PlayOneShot(SoundInfo.InMap(this.parent.pawn, MaintenanceType.None));
            }
           
            
            base.CompPostMake();
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            
            if (chocke_int != 1)
            {
                --chocke_int;
            }
            if (Rand.Chance(0.55f))
            {
                float change = 0.02f;
                if(source != null && source.BleedRate > 0.01f)
                {
                    change = source.BleedRate / 5;
                    //////log.Message(change.ToString());
                }
                else
                {
                    change = 0.10f;
                }
                if (chocke_int == 1)
                {
                    if (Settings.somesound)
                    {
                        this.Props.coughSound.PlayOneShot(SoundInfo.InMap(this.parent.pawn, MaintenanceType.None));
                    }
                    this.parent.Severity += change;
                    chocke_int = Props.ABCD;
                }
            }
            else
            {
                if (chocke_int == 1)
                {
                    float change = 0.25f;
                    if (Settings.somesound)
                    {
                        this.Props.coughSound.PlayOneShot(SoundInfo.InMap(this.parent.pawn, MaintenanceType.None));
                    }
                    if (source.IsTended())
                    {
                        change = 0.11f;
                    }
                    this.parent.Severity -= change;
                    chocke_int = Props.ABCD;
                }
            }
          
            base.CompPostTick(ref severityAdjustment);
        }


        public chokingcompProperties Props => (chokingcompProperties)this.props;

       
    }

    public class chokingcompProperties : HediffCompProperties
    {
        public int ABCD;
        public SoundDef coughSound;

       
        public chokingcompProperties()
        {
            this.compClass = typeof(chokingcomp);
        }

        public chokingcompProperties(Type compClass) : base()
        {
            this.compClass = compClass;
        }
    }
	internal class ClearAirway : JobDriver
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

		
		public Thing suctiondevice
		{
			get
			{
				return this.job.targetB.Thing;
			}
		}

		
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.Doctor.Reserve(this.Patient, this.job, 1, -1, null, errorOnFailed);
		}

		
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnAggroMentalState(TargetIndex.A);
			
			Toil toil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return toil;
			Toil toil2 = Toils_General.Wait(320);
			toil2.AddFinishAction(delegate
			{
				Patient.health.RemoveHediff(Patient.health.hediffSet.hediffs.Find(AAA => AAA.def == Caula_DefOf.ChokingOnBlood));
			});
			yield return toil2;
			yield break;
		}

		
		private const TargetIndex targetInd = TargetIndex.A;
	}
    public class fixlung : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            usedBy.health.hediffSet.hediffs.RemoveAll(async => async.def == Caula_DefOf.ChokingOnBlood);

        }
    }
}
