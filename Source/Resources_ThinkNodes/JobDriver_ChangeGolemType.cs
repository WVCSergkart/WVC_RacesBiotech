using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_ChangeGolemType : JobDriver
	{


		public Thing Target => job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			CompGolem golemComp = pawn.TryGetComp<CompGolem>();
			if (golemComp == null)
			{
				yield break;
			}
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 320, useProgressBar: true).WithEffect(EffecterDefOf.ControlMech, TargetIndex.A);
			yield return Toils_General.Do(delegate
			{
				PawnKindDef newPawn = golemComp.targetMode.pawnKindDef;
				Pawn overseer = pawn.GetOverseer();
				if (newPawn == null || overseer == null)
				{
					return;
				}
				if (Gene_Golemlink.TryCreateGolemFromThing(Target, newPawn, overseer))
				{
					pawn.forceNoDeathNotification = true;
					pawn.Kill(null);
					pawn.forceNoDeathNotification = false;
				}
			});
		}

	}

}
