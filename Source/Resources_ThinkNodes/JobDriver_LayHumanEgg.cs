using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
	//public class JobDriver_LayHumanEgg : JobDriver_XaGJob_General
	//{

	//	public IntVec3 Target => job.targetA.Cell;

	//	public override bool TryMakePreToilReservations(bool errorOnFailed)
	//	{
	//		return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
	//	}

	//	protected override IEnumerable<Toil> MakeNewToils()
	//	{
	//		this.FailOnForbidden(TargetIndex.A);
	//		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
	//		yield return Toils_General.WaitWith(TargetIndex.A, 620, useProgressBar: true).WithEffect(EffecterDefOf.Birth, TargetIndex.A);
	//		yield return Toils_General.Do(delegate
	//		{
	//			if (job is not XaG_Job xaG_Job || xaG_Job.gene == null || xaG_Job.gene is not Gene_Ovipositor ovipositor)
	//			{
	//				return;
	//			}
	//			ovipositor.LayEgg(pawn.health.hediffSet.GetFirstHediff<Hediff_Pregnant>());
	//		});
	//	}

	//}
	public class JobDriver_LayHumanEgg : JobDriver_XaGJob_General
	{

		public CompEggContainer EggBoxComp => job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompEggContainer>();

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
			yield return Toils_General.WaitWith(TargetIndex.A, 620, useProgressBar: true).WithEffect(EffecterDefOf.Birth, TargetIndex.A);
			yield return Toils_General.Do(delegate
			{
				if (job is not XaG_Job xaG_Job || xaG_Job.gene == null || xaG_Job.gene is not Gene_Ovipositor ovipositor)
				{
					Log.Warning("Wrong job/gene. Egg laying impossible.");
					return;
				}
				ovipositor.LayEgg(pawn.health.hediffSet.GetFirstHediff<Hediff_Pregnant>());
			});
		}
	}

}
