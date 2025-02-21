using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_DevourFleshmassNucleus : JobDriver_XaGJob_General
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
			yield return Toils_General.WaitWith(TargetIndex.A, 720, useProgressBar: true).WithEffect(EffecterDefOf.ControlMech, TargetIndex.A);
			yield return Toils_General.Do(delegate
			{
				if (job is XaG_Job xaG_Job)
                {
					//XaG_GeneUtility.AddGeneToChimera(pawn, geneDef);
					XaG_GeneUtility.ImplantChimeraEvolveGeneSet(pawn, geneDef);
					GeneResourceUtility.OffsetNeedFood(pawn, 100, true);
				}
                //HediffUtility.MutationMeatSplatter(Victim, false);
				Victim.Destroy();
			});
		}

	}

}
