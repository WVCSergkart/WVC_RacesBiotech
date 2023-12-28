using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_RetuneSerum : JobDriver
	{

		public int ticksToRetune = 180;

		public Thing Target => job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			CompUseEffect_XenotypeForcer_II xenotypeForcer = Target.TryGetComp<CompUseEffect_XenotypeForcer_II>();
			if (xenotypeForcer == null)
			{
				yield break;
			}
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, ticksToRetune, useProgressBar: true);
			yield return Toils_General.Do(delegate
			{
				Find.WindowStack.Add(new Dialog_RetuneSerum(Target));
			});
		}

	}

}
