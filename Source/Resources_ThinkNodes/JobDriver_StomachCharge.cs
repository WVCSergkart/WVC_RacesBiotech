using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_StomachCharge : JobDriver, IJobCustomEater
	{
		// private const TargetIndex ChargerInd = TargetIndex.A;
		public virtual bool ShouldFinalize => true;

		public Building_XenoCharger Charger => (Building_XenoCharger)job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (!pawn.Reserve(Charger, job, 1, -1, null, errorOnFailed))
			{
				return false;
			}
			return true;
		}

		// public Gene_RechargeableStomach energyGene;

		protected override IEnumerable<Toil> MakeNewToils()
		{
			// if (energyGene == null)
			// {
			// energyGene = pawn?.genes?.GetFirstGeneOfType<Gene_RechargeableStomach>();
			// }
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => !Charger.CanPawnChargeCurrently(pawn));
			yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.InteractionCell).FailOnForbidden(TargetIndex.A);
			Toil toil = ToilMaker.MakeToil("MakeNewToils");
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.initAction = delegate
			{
				Charger.StartCharging(pawn);
			};
			toil.AddFinishAction(delegate
			{
				Charger.StopCharging();
			});
			toil.handlingFacing = true;
			toil.tickAction = (Action)Delegate.Combine(toil.tickAction, (Action)delegate
			{
				pawn.rotationTracker.FaceTarget(Charger.Position);
				if (pawn.needs.food.CurLevel >= pawn.needs.food.MaxLevel)
				{
					ReadyForNextToil();
				}
			});
			yield return toil;
		}
	}

	public class JobDriver_HemogenCharge : JobDriver_StomachCharge
	{

		private Gene_Hemogen cachedHemogenGene;

		public Gene_Hemogen Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		public override bool ShouldFinalize => false;

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => !Charger.CanPawnChargeCurrently(pawn) && Hemogen == null);
			yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.InteractionCell).FailOnForbidden(TargetIndex.A);
			Toil toil = ToilMaker.MakeToil("MakeNewToils");
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.initAction = delegate
			{
				Charger.StartCharging(pawn);
			};
			toil.AddFinishAction(delegate
			{
				Charger.StopCharging();
			});
			toil.handlingFacing = true;
			toil.tickAction = (Action)Delegate.Combine(toil.tickAction, (Action)delegate
			{
				pawn.rotationTracker.FaceTarget(Charger.Position);
				if (Hemogen.Value >= Hemogen.Max)
				{
					ReadyForNextToil();
				}
			});
			yield return toil;
		}

	}

	public class JobDriver_RepairHuman : JobDriver
	{

		private Gene_SelfRepair cachedRepairGene;

		public Gene_SelfRepair Gene
		{
			get
			{
				if (cachedRepairGene == null || !cachedRepairGene.Active)
				{
					cachedRepairGene = Mech?.genes?.GetFirstGeneOfType<Gene_SelfRepair>();
				}
				return cachedRepairGene;
			}
		}

		protected int ticksToNextRepair;

		protected Pawn Mech => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		protected virtual bool Remote => false;

		protected int TicksPerHeal => Mathf.RoundToInt(1f / pawn.GetStatValue(StatDefOf.MechRepairSpeed) * 120f);

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Mech, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnForbidden(TargetIndex.A);
			this.FailOn(() => Mech.IsAttacking() || Gene == null);
			if (!Remote)
			{
				yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			}
			Toil toil = (Remote ? Toils_General.Wait(int.MaxValue) : Toils_General.WaitWith(TargetIndex.A, int.MaxValue, useProgressBar: false, maintainPosture: true, maintainSleep: true));
			toil.WithEffect(EffecterDefOf.MechRepairing, TargetIndex.A);
			toil.PlaySustainerOrSound(Remote ? SoundDefOf.RepairMech_Remote : SoundDefOf.RepairMech_Touch);
			toil.AddPreInitAction(delegate
			{
				ticksToNextRepair = TicksPerHeal;
			});
			toil.handlingFacing = true;
			toil.tickAction = delegate
			{
				ticksToNextRepair--;
				if (ticksToNextRepair <= 0)
				{
					Gene.Notify_RepairedBy(pawn, TicksPerHeal);
					ticksToNextRepair = TicksPerHeal;
				}
				pawn.rotationTracker.FaceTarget(Mech);
				if (pawn.skills != null)
				{
					pawn.skills.Learn(SkillDefOf.Crafting, 0.05f);
				}
			};
			toil.AddFinishAction(delegate
			{
				if (Mech.jobs?.curJob != null)
				{
					Mech.jobs.EndCurrentJob(JobCondition.InterruptForced);
				}
			});
			toil.AddEndCondition(() => (Mech.health.summaryHealth.SummaryHealthPercent < 1f) ? JobCondition.Ongoing : JobCondition.Succeeded);
			if (!Remote)
			{
				toil.activeSkill = () => SkillDefOf.Crafting;
			}
			yield return toil;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToNextRepair, "ticksToNextRepair", 0);
		}
	}

}
