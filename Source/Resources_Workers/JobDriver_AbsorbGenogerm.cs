using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class JobDriver_AbsorbGenogerm : JobDriver
	{

		// public int ticksToAbsorb = 60;
		// public bool reimplantEndogenes = true;
		// public bool reimplantXenogenes = true;

		public Pawn Target => job.targetA.Thing as Pawn;

		private Mote warmupMote;
		private TargetInfo verbTargetInfoTmp = null;
		private Effecter warmupEffecter;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (!ModLister.CheckBiotech("xenogerm absorbing"))
			{
				yield break;
			}
			JobExtension_Reimplanter jobExtension = job.def?.GetModExtension<JobExtension_Reimplanter>();
			if (jobExtension == null)
			{
				yield break;
			}
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			// yield return Toils_General.WaitWith(TargetIndex.A, jobExtension.ticksToAbsorb, useProgressBar: true);
			Toil toil = Toils_General.Wait(jobExtension.ticksToAbsorb);
			toil.WithProgressBarToilDelay(TargetIndex.A);
			toil.FailOnDespawnedOrNull(TargetIndex.A);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			toil.tickAction = delegate
			{
				// if (jobExtension.warmupStartSound != null)
				// {
					// jobExtension.warmupStartSound.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
				// }
				Thing thing = Target;
				if (warmupEffecter == null && jobExtension.warmupEffecter != null)
				{
					if (thing != null)
					{
						warmupEffecter = jobExtension.warmupEffecter.SpawnAttached(thing, pawn.MapHeld);
						verbTargetInfoTmp = thing;
					}
					else
					{
						warmupEffecter = jobExtension.warmupEffecter.Spawn(Target.Position, pawn.MapHeld);
						verbTargetInfoTmp = new TargetInfo(Target.Position, pawn.MapHeld);
					}
					warmupEffecter.Trigger(verbTargetInfoTmp, verbTargetInfoTmp);
				}
				warmupEffecter?.EffectTick(verbTargetInfoTmp, verbTargetInfoTmp);
				warmupMote = MoteMaker.MakeAttachedOverlay(Target, ThingDefOf.Mote_XenogermImplantation, Vector3.zero);
				if (jobExtension.warmupMote != null)
				{
					warmupMote = MoteMaker.MakeAttachedOverlay(Target, jobExtension.warmupMote, Vector3.zero);
				}
				warmupMote?.Maintain();
			};
			yield return toil;
			yield return Toils_General.Do(delegate
			{
				if (jobExtension.warmupStartSound != null)
				{
					jobExtension.warmupStartSound.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
				}
				if (Target.HomeFaction != null && pawn.Faction == Faction.OfPlayer)
				{
					Faction.OfPlayer.TryAffectGoodwillWith(Target.HomeFaction, -50, canSendMessage: true, !Target.HomeFaction.temporary, HistoryEventDefOf.AbsorbedXenogerm);
				}
				QuestUtility.SendQuestTargetSignals(Target.questTags, "XenogermAbsorbed", Target.Named("SUBJECT"));
				ReimplanterUtility.Reimplanter(Target, pawn, jobExtension.reimplantEndogenes, jobExtension.reimplantXenogenes);
				// if (PawnUtility.ShouldSendNotificationAbout(pawn))
				// {
					// int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					// int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					// Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(Target.Named("CASTER"), pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
				// }
			});
		}

	}

}
