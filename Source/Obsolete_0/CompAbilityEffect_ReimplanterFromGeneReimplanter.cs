using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompAbilityEffect_ReimplanterFromGeneReimplanter : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		[Unsaved(false)]
		private Gene_XenotypeImplanter cachedReimplanterGene;

		public Gene_XenotypeImplanter ReimplanterGene
		{
			get
			{
				if (cachedReimplanterGene == null || !cachedReimplanterGene.Active)
				{
					cachedReimplanterGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_XenotypeImplanter>();
				}
				return cachedReimplanterGene;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, ReimplanterGene.xenotypeDef);
				//pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
				//GeneUtility.UpdateXenogermReplication(pawn);
				ReimplanterUtility.UpdateXenogermReplication_WithComa(pawn);
				ReimplanterUtility.ExtractXenogerm(parent.pawn);
				ReimplanterUtility.FleckAndLetter(parent.pawn, pawn);
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			//Pawn pawn = target.Pawn;
			//if (pawn == null)
			//{
			//	return base.Valid(target, throwMessages);
			//}
			//if (pawn.IsQuestLodger())
			//{
			//	if (throwMessages)
			//	{
			//		Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
			//	}
			//	return false;
			//}
			//if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
			//{
			//	if (throwMessages)
			//	{
			//		Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
			//	}
			//	return false;
			//}
			//if (!PawnIdeoCanAcceptReimplant(pawn))
			//{
			//	if (throwMessages)
			//	{
			//		Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
			//	}
			//	return false;
			//}
			//if (ReimplanterGene == null || !pawn.IsHuman())
			//{
			//	if (throwMessages)
			//	{
			//		Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
			//	}
			//	return false;
			//}
			//return base.Valid(target, throwMessages);
			return ReimplanterUtility.ImplanterValidation(parent.pawn, target, throwMessages) && base.Valid(target, throwMessages);
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

		//public bool PawnIdeoCanAcceptReimplant(Pawn implantee)
		//{
		//	if (!ModsConfig.IdeologyActive)
		//	{
		//		return true;
		//	}
		//	if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.BecomeNonPreferredXenotype, implantee) && !implantee.Ideo.PreferredXenotypes.Contains(ReimplanterGene.xenotypeDef))
		//	{
		//		return false;
		//	}
		//	return true;
		//}

	}
}
