using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_DevourFleshmassNucleus : JobDriver
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
				yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true).PlaySustainerOrSound(WVC_GenesDefOf.RevenantSpineSmash);
			}
			else
			{
				yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true);
			}
			yield return Toils_General.Do(delegate
			{
				if (job is XaG_Job xaG_Job)
				{
					XaG_GeneUtility.AddGenesToChimera(pawn, new() { xaG_Job.geneDef });
					Messages.Message("WVC_XaG_GeneGeneticThief_GeneObtained".Translate(pawn.NameShortColored, xaG_Job.geneDef.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				HediffUtility.MutationMeatSplatter(Victim, false);
				Victim.Kill(null);
			});
		}

	}

}
