using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityReimplanter : CompProperties_AbilityEffect
	{

		public ThoughtDef afterResurrectionThoughtDef;

		public ThoughtDef resurrectorThoughtDef;
		public ThoughtDef resurrectedThoughtDef;

		public JobDef absorberJob;

		public List<GeneDef> geneDefs;

		public List<GeneDef> inheritableGenes;

		public bool reimplantEndogenes = true;
		public bool reimplantXenogenes = true;

		public CompProperties_AbilityReimplanter()
		{
			compClass = typeof(CompAbilityEffect_Reimplanter);
		}

	}

	public class CompAbilityEffect_Reimplanter : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				if (ReimplanterUtility.TryReimplant(parent.pawn, pawn, Props.reimplantEndogenes, Props.reimplantXenogenes))
				{
					FleckMaker.AttachedOverlay(pawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
					if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(pawn))
					{
						int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
						int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
						Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, pawn));
					}
				}
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return base.Valid(target, throwMessages);
			}
			if (pawn.IsQuestLodger())
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
			{
				if (throwMessages)
				{
					Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (!PawnIdeoCanAcceptReimplant(parent.pawn, pawn))
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (!pawn.IsHuman())
			{
				if (throwMessages)
				{
					Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
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
			Pawn pawn = target.Pawn;
			yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

		public static bool PawnIdeoCanAcceptReimplant(Pawn implanter, Pawn implantee)
		{
			if (implantee.Downed)
			{
				return true;
			}
			return CompAbilityEffect_ReimplantXenogerm.PawnIdeoCanAcceptReimplant(implanter, implantee);
		}

	}
}
