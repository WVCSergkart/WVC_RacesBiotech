using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompProperties_AbilityRiseFromTheDead : CompProperties_AbilityEffect
	{

		public ThoughtDef afterResurrectionThoughtDef;

		public ThoughtDef resurrectorThoughtDef;
		public ThoughtDef resurrectedThoughtDef;

		public CompProperties_AbilityRiseFromTheDead()
		{
			compClass = typeof(CompAbilityEffect_RiseFromTheDead);
		}
	}

	public class CompAbilityEffect_RiseFromTheDead : CompAbilityEffect
	{
		// private static readonly CachedTexture ReimplantIcon = new CachedTexture("WVC/UI/Genes/Reimplanter");

		private new CompProperties_AbilityRiseFromTheDead Props => (CompProperties_AbilityRiseFromTheDead)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			// base.Apply(target, dest);
			// ResurrectionUtility.Resurrect(target);
			base.Apply(target, dest);
			Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
			UndeadUtility.ResurrectWithSickness(innerPawn, Props.afterResurrectionThoughtDef);
			// ResurrectionUtility.Resurrect(innerPawn);
			// innerPawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
			// innerPawn.needs?.mood?.thoughts?.memories.TryGainMemory(WVC_GenesDefOf.WVC_XenotypesAndGenes_WasResurrected);
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
			// Pawn pawn = target.Pawn;
			if (innerPawn != null)
			{
				MechanoidizationUtility.ReimplantEndogerm(parent.pawn, innerPawn);
				FleckMaker.AttachedOverlay(innerPawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
				if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(innerPawn))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("WVC_LetterLabel_GeneRiseFromTheDead".Translate(), "WVC_LetterText_GeneRiseFromTheDead".Translate(innerPawn.Named("TARGET")) + "\n\n" + "LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), innerPawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, innerPawn));
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
				Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
				if (MechanoidizationUtility.PawnIsAndroid(innerPawn) || MechanoidizationUtility.PawnCannotUseSerums(innerPawn))
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
			return null;
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
			yield return MoteMaker.MakeAttachedOverlay(innerPawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}
	}
}
