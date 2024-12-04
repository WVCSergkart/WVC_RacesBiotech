using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_RiseFromTheDead : CompAbilityEffect
	{

		private new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
			GeneResourceUtility.ResurrectWithSickness(innerPawn, Props.afterResurrectionThoughtDef);
			//if ((innerPawn.Faction == null || innerPawn.Faction != Faction.OfPlayer) && innerPawn.guest.Recruitable)
			//{
			//	RecruitUtility.Recruit(innerPawn, Faction.OfPlayer, parent.pawn);
			//	Messages.Message("WVC_XaG_ReimplantResurrectionRecruiting".Translate(innerPawn), innerPawn, MessageTypeDefOf.PositiveEvent);
			//}
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_HistoryEventDefDefOf.WVC_ReimplanterResurrection, parent.pawn.Named(HistoryEventArgsNames.Doer)));
			}
			if (Props.resurrectedThoughtDef != null)
			{
				innerPawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.resurrectedThoughtDef, parent.pawn);
			}
			if (Props.resurrectorThoughtDef != null)
			{
				parent.pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.resurrectorThoughtDef, innerPawn);
			}
			Messages.Message("MessagePawnResurrected".Translate(innerPawn), innerPawn, MessageTypeDefOf.PositiveEvent);
			MoteMaker.MakeAttachedOverlay(innerPawn, ThingDefOf.Mote_ResurrectFlash, Vector3.zero);
			if (innerPawn != null)
			{
				if (ReimplanterUtility.TryReimplant(parent.pawn, innerPawn, Props.reimplantEndogenes, Props.reimplantXenogenes))
				{
					FleckMaker.AttachedOverlay(innerPawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
					if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(innerPawn))
					{
						int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
						int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
						Find.LetterStack.ReceiveLetter("WVC_LetterLabel_GeneRiseFromTheDead".Translate(), "WVC_LetterText_GeneRiseFromTheDead".Translate(innerPawn.Named("TARGET")) + "\n\n" + "LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), innerPawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, innerPawn));
					}
				}
				foreach (Gene gene in parent.pawn.genes.GenesListForReading)
				{
					if (gene is Gene_PostImplanterDependant postgene && gene.Active)
					{
						postgene.Notify_TargetResurrected(innerPawn);
					}
				}
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				if (corpse.GetRotStage() == RotStage.Dessicated)
				{
					if (throwMessages)
					{
						Messages.Message("MessageCannotResurrectDessicatedCorpse".Translate(), corpse, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				Pawn innerPawn = corpse.InnerPawn;
				if (!innerPawn.IsHuman() || innerPawn.IsMutant)
				{
					if (throwMessages)
					{
						Messages.Message("WVC_PawnIsAndroidCheck".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
			}
			return base.Valid(target, throwMessages);
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			if (GeneUtility.PawnWouldDieFromReimplanting(parent.pawn))
			{
				return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
			}
			if (target.Thing is Corpse corpse && !corpse.InnerPawn.guest.Recruitable && (corpse.InnerPawn.Faction == null || corpse.InnerPawn.Faction != Faction.OfPlayer))
			{
				return Dialog_MessageBox.CreateConfirmation("WVC_XaG_ReimplantResurrectionRecruiting_FailWarning".Translate(corpse.InnerPawn.Named("PAWN"), corpse.InnerPawn.Faction.NameColored.ToString()), confirmAction);
			}
			return null;
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			yield return MoteMaker.MakeAttachedOverlay((Corpse)target.Thing, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}
	}
}
