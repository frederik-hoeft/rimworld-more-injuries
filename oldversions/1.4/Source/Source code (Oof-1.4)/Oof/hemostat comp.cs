using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Oof
{
    [DefOf]
    public class HemoDefOf : DefOf
    {
		public static HediffDef hemostatised;

		public static JobDef UseHemo;
	}


    public class hemostat_modext : DefModExtension
    {
        public float coagulation_mult;

		public int applytime;
    }

    public class hemostat_comp : ThingComp
    {
        public float tagged_float;

		public BetterInjury injur;

		public List<BetterInjury> injuries_coagulateable;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
			if (OofMod.settings.individualFloatMenus)
			{
				if (selPawn.inventory.innerContainer.Any(iran => iran.def.HasModExtension<hemostat_modext>()))
				{

					Pawn parent = this.parent as Pawn;

					var hemo = selPawn.inventory.innerContainer.ToList().Find(iran => iran.def.HasModExtension<hemostat_modext>());

					var hemoprops = hemo.def.GetModExtension<hemostat_modext>();

					foreach (BetterInjury injury in parent.health.hediffSet.GetInjuriesTendable())
					{
						if (injury.Part != null && injury.Bleeding && !injury.hemod && injury.Part.depth == BodyPartDepth.Outside)
						{
							float newbleedrate = injury.Severity * injury.def.injuryProps.bleedRate * injury.Part.def.bleedRate * hemoprops.coagulation_mult;

							tagged_float = newbleedrate;

							yield return new FloatMenuOption("Use " + hemo.Label + " on: ".Colorize(new Color(26, 49, 20)) + injury.Label + " on " + injury.Part.Label.Colorize(new Color(26, 49, 20)),
								delegate
								{
									injur = injury;

									Job jobber = new Job { def = HemoDefOf.UseHemo, targetA = parent, targetB = selPawn.inventory.innerContainer.ToList().Find(iran => iran.def.HasModExtension<hemostat_modext>()) };

									selPawn.jobs.StartJob(jobber, JobCondition.InterruptForced);
								}

								);
						}

					}
				}
			}

			if (selPawn.inventory.innerContainer.Any(x => x.def.HasModExtension<hemostat_modext>()) | !selPawn.ThingsInRange().Where(x => x.def.HasModExtension<hemostat_modext>()).EnumerableNullOrEmpty()
				&& !((Pawn)this.parent).health.hediffSet.GetInjuriesTendable().EnumerableNullOrEmpty())
			{
				yield return new FloatMenuOption("Provide first aid",
								delegate
								{
									selPawn.jobs.StartJob(new Job { def = FirstAidJobDefOf.FirstAid, targetA = this.parent }, JobCondition.InterruptForced);
								});
			}

			
        }

    }

	public class UseHemostat : JobDriver
	{

		public Pawn Patient
		{
			get
			{
				return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}


		public Pawn Doctor
		{
			get
			{
				return this.pawn;
			}
		}



		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.Doctor.Reserve(this.Patient, this.job, 1, 1, null, errorOnFailed);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnAggroMentalState(TargetIndex.A);
			Toil toil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return toil;

			var varA1 = TargetB.Thing.def.GetModExtension<hemostat_modext>();

			Toil toil2 = Toils_General.Wait(varA1.applytime);
			toil2.AddFinishAction(delegate
			{
				var varC = Patient.TryGetComp<hemostat_comp>().injur;

				varC.isBase = false;

				varC.BleedRateSet = Patient.TryGetComp<hemostat_comp>().tagged_float;

				//Hediff hedf = HediffMaker.MakeHediff(HemoDefOf.hemostatised, Patient, varC.Part);

				//hedf.Severity = 1f;

				//hedf.TryGetComp<HemoHefComp>().injur = varC;

				//Patient.health.AddHediff(hedf);

				varC.hemod = true;

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
			});
			yield return toil2;
			yield break;
		}
	}

	public class HemoHefComp : HediffComp
	{
		public BetterInjury injur;

		public override void CompPostPostRemoved()
		{
			if (injur != null)
			{
				injur.isBase = true;

				injur.hemod = false;
			}
			base.CompPostPostRemoved();
		}


	}
}
