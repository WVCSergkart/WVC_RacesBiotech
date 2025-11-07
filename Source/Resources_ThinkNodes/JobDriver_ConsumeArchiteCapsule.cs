using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
	public class JobDriver_ConsumeArchiteCapsule : JobDriver_XaGJob_General
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
				Gene_Chimera chimera = pawn.genes?.GetFirstGeneOfType<Gene_Chimera>();
				if (chimera != null)
				{
					chimera?.AddArchiteLimit();
					//chimera?.DoEffects(pawn);
					if (!chimera.Props.soundDefOnImplant.NullOrUndefined())
					{
						chimera.Props.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
					}
					Target.ReduceStack();
				}
			});
		}

	}

	public class JobDriver_ConsumeArchiteCapsule_All : JobDriver_ConsumeArchiteCapsule
	{

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 320, useProgressBar: true).WithEffect(EffecterDefOf.ButcherMechanoid, TargetIndex.A);
			yield return Toils_General.Do(delegate
			{
				Gene_Chimera chimera = pawn.genes?.GetFirstGeneOfType<Gene_Chimera>();
				if (chimera != null)
				{
					chimera?.AddArchiteLimit(Target.stackCount);
					//chimera?.DoEffects(pawn);
					if (!chimera.Props.soundDefOnImplant.NullOrUndefined())
					{
						chimera.Props.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
					}
					Target.Destroy();
				}
			});
		}

	}

}
