using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class XaG_Job : Job, IExposable, ILoadReferenceable
    {

		public XaG_Job(Job job)
		{
			this.def = job.def;
			this.targetA = job.targetA;
		}

		public GeneDef geneDef;

    }

    public class JobDriver_ChangeMorpherTriggerGene : JobDriver
	{

		public Thing Target => job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 320, useProgressBar: true).WithEffect(EffecterDefOf.ButcherMechanoid, TargetIndex.A);
			yield return Toils_General.Do(delegate
            {
                if (Target.stackCount > 1)
                {
                    Target.stackCount -= 1;
                }
				else
                {
					Target.Destroy();
				}
				if (job is XaG_Job xaG_Job)
				{
					Gene_Morpher morpher = pawn.genes?.GetFirstGeneOfType<Gene_Morpher>();
					morpher?.UpdToolGenes(true, xaG_Job.geneDef);
					morpher?.DoEffects();
				}
			});
		}

	}

}
