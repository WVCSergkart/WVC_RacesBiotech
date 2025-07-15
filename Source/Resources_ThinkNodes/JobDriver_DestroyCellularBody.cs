using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_DestroyCellularBody : JobDriver
	{

		public Pawn Victim => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			if (!ModLister.CheckAnomaly("Revenant"))
			{
				yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true).PlaySustainerOrSound(MainDefOf.RevenantSpineSmash);
			}
			else
			{
				yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true);
			}
			yield return Toils_General.Do(delegate
			{
				Victim.genes?.GetFirstGeneOfType<Gene_Cellular>()?.ExtractShard(pawn);
			});
		}

	}

}
