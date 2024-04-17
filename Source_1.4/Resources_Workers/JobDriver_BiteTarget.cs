using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_BiteTarget : JobDriver
	{

		public Pawn Target => job.targetA.Thing as Pawn;

		private Mote warmupMote;
		private TargetInfo verbTargetInfoTmp = null;
		private Effecter warmupEffecter;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobExtension_Reimplanter jobExtension = job.def?.GetModExtension<JobExtension_Reimplanter>();
			if (jobExtension == null)
			{
				yield break;
			}
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			// yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true);
			Toil toil = Toils_General.Wait(420);
			toil.WithProgressBarToilDelay(TargetIndex.A);
			toil.FailOnDespawnedOrNull(TargetIndex.A);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			toil.tickAction = delegate
			{
				Pawn target = Target;
				if (warmupEffecter == null && jobExtension.warmupEffecter != null)
				{
					if (target != null)
					{
						warmupEffecter = jobExtension.warmupEffecter.SpawnAttached(target, pawn.MapHeld);
						verbTargetInfoTmp = target;
					}
					else
					{
						warmupEffecter = jobExtension.warmupEffecter.Spawn(target.Position, pawn.MapHeld);
						verbTargetInfoTmp = new TargetInfo(target.Position, pawn.MapHeld);
					}
					warmupEffecter.Trigger(verbTargetInfoTmp, verbTargetInfoTmp);
				}
				warmupEffecter?.EffectTick(verbTargetInfoTmp, verbTargetInfoTmp);
				warmupMote = MoteMaker.MakeAttachedOverlay(target, ThingDefOf.Mote_XenogermImplantation, Vector3.zero);
				if (jobExtension.warmupMote != null)
				{
					warmupMote = MoteMaker.MakeAttachedOverlay(target, jobExtension.warmupMote, Vector3.zero);
				}
				warmupMote?.Maintain();
			};
			yield return toil;
			yield return Toils_General.Do(delegate
			{
				SanguophageUtility.DoBite(pawn, Target, 0.33f, 0.11f, 0.44f, 1f, new(1, 1), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
			});
		}

	}

}
