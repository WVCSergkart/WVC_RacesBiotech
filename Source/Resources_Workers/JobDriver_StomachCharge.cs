using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_StomachCharge : JobDriver
	{
		// private const TargetIndex ChargerInd = TargetIndex.A;

		public Building_MechCharger Charger => (Building_MechCharger)job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (!pawn.Reserve(Charger, job, 1, -1, null, errorOnFailed))
			{
				return false;
			}
			return true;
		}

		public Gene_RechargeableStomach energyGene;

		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (energyGene == null)
			{
				energyGene = pawn?.genes?.GetFirstGeneOfType<Gene_RechargeableStomach>();
			}
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => energyGene == null || !Gene_RechargeableStomach.CanPawnChargeCurrently(Charger));
			yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.InteractionCell).FailOnForbidden(TargetIndex.A);
			Toil toil = ToilMaker.MakeToil("MakeNewToils");
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.initAction = delegate
			{
				energyGene.StartCharging(Charger);
			};
			toil.AddFinishAction(delegate
			{
				energyGene.StopCharging();
			});
			toil.handlingFacing = true;
			toil.tickAction = (Action)Delegate.Combine(toil.tickAction, (Action)delegate
			{
				pawn.rotationTracker.FaceTarget(Charger.Position);
				if (pawn.needs.food.CurLevelPercentage >= pawn.needs.food.MaxLevel - 0.06f)
				{
					ReadyForNextToil();
				}
			});
			yield return toil;
		}
	}

}
