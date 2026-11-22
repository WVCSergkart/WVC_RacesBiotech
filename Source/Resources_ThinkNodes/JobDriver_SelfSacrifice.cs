using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class JobDriver_SelfSacrifice : JobDriver_XaGJob_General
	{

		public Thing Target => job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.Do(delegate
			{
				if (Target is Pawn holder)
				{
					holder.genes?.GetFirstGeneOfType<Gene_Sacrificer>()?.SelfSactifice(pawn);
				}
			});
		}

	}

}
