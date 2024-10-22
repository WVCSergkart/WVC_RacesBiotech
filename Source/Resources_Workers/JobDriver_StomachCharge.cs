using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_StomachCharge : JobDriver, IJobCustomEater
	{
		// private const TargetIndex ChargerInd = TargetIndex.A;
		public bool ShouldFinalize => true;

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

}
