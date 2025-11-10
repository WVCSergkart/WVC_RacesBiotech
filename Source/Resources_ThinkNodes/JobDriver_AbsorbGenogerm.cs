using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_AbsorbTargetGenes : JobDriver_XaGJob_General
	{

		public Pawn Target => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 60, useProgressBar: true);
			yield return Toils_General.Do(delegate
			{
				if (Target.HomeFaction != null && pawn.Faction == Faction.OfPlayer)
				{
					Faction.OfPlayer.TryAffectGoodwillWith(Target.HomeFaction, -50, canSendMessage: true, !Target.HomeFaction.temporary, RimWorld.HistoryEventDefOf.AbsorbedXenogerm);
				}
				try
				{
					QuestUtility.SendQuestTargetSignals(Target.questTags, "XenogermAbsorbed", Target.Named("SUBJECT"));
					ReimplanterUtility.TryReimplant(Target, pawn, implantEndogenes, implantXenogenes);
				}
				catch (Exception arg)
				{
					Log.Error("Failed absorb genes. Reason: " + arg.Message);
				}
			});
		}

	}

	[Obsolete]
	public class JobDriver_AbsorbGenogerm : JobDriver_AbsorbTargetGenes
	{

		protected override IEnumerable<Toil> MakeNewToils()
		{
			GeneExtension_General jobExtension = job.def?.GetModExtension<GeneExtension_General>();
			if (jobExtension == null)
			{
				yield break;
			}
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 60, useProgressBar: true);
			yield return Toils_General.Do(delegate
			{
				if (Target.HomeFaction != null && pawn.Faction == Faction.OfPlayer)
				{
					Faction.OfPlayer.TryAffectGoodwillWith(Target.HomeFaction, -50, canSendMessage: true, !Target.HomeFaction.temporary, RimWorld.HistoryEventDefOf.AbsorbedXenogerm);
				}
				QuestUtility.SendQuestTargetSignals(Target.questTags, "XenogermAbsorbed", Target.Named("SUBJECT"));
				if (ReimplanterUtility.TryReimplant(Target, pawn, jobExtension.reimplantEndogenes, jobExtension.reimplantXenogenes))
				{
					if (jobExtension.warmupStartSound != null)
					{
						jobExtension.warmupStartSound.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
					}
				}
			});
		}

	}

}
