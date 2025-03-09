using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_StorageImplanter : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		[Unsaved(false)]
		private Gene_StorageImplanter cachedGene;
		public Gene_StorageImplanter Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_StorageImplanter>();
				}
				return cachedGene;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				ReimplanterUtility.SetXenotype(pawn, Gene.XenotypeHolder);
				ReimplanterUtility.UpdateXenogermReplication_WithComa(pawn);
				ReimplanterUtility.ExtractXenogerm(parent.pawn);
				ReimplanterUtility.FleckAndLetter(parent.pawn, pawn);
				Gene.ResetHolder();
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (Gene?.XenotypeHolder == null)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_StorageImplanter_NonValidMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
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

		public override string ExtraTooltipPart()
		{
			return "WVC_XaG_StorageImplanter_ExtraTooltip".Translate(Gene.XenotypeHolder != null ? Gene.XenotypeHolder.LabelCap : " - ", Gene.XenotypeHolder != null ? Gene.XenotypeHolder.genes.Count : "0").ToString();
		}

	}

}
