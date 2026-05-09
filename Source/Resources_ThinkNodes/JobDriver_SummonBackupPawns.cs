using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class JobDriver_SummonBackupPawns : JobDriver_XaGJob_General
	{

		public Thing Victim =>job.GetTarget(TargetIndex.A).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			yield return Toils_General.WaitWith(TargetIndex.A, 320, useProgressBar: true).WithEffect(EffecterDefOf.ControlMech, TargetIndex.A);
			yield return Toils_General.Do(delegate
			{
				Victim?.TryGetComp<CompBackupSummonPawns>()?.InitSummon();
			});
		}

	}

}
