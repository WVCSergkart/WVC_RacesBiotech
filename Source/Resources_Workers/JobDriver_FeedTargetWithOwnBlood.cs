using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_FeedTargetWithOwnBlood : JobDriver
	{

		public Pawn Victim => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOn(() => pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss));
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true).PlaySustainerOrSound(SoundDefOf.Bloodfeed_Cast);
			yield return Toils_General.Do(delegate
			{
				// SanguophageUtility.DoBite(Victim, pawn, 0.1f, 0.1f, 0.2f, 0f, new(0, 1));
				// SanguophageUtility.DoBite(Victim, pawn, 0.1f, 0.05f, 0.2245f, 0f, IntRange.one, ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
				SanguophageUtility.DoBite(Victim, pawn, 0.2f, 0.1f, 0.4499f, 1f, new(0, 1), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
			});
		}

		// public override void Notify_Starting()
		// {
			// base.Notify_Starting();
			// job.ability?.Notify_StartedCasting();
		// }

	}


}
