using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class CompAbilityEffect_HybridImplanter : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		[Unsaved(false)]
		private Gene_HybridImplanter cachedGene;
		public Gene_HybridImplanter Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_HybridImplanter>();
				}
				return cachedGene;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn victim = target.Pawn;
			if (victim != null)
			{
                Pawn caster = parent.pawn;
                if (SubXenotypeUtility.TrySetHybridXenotype(caster, victim, Gene, Gene.IsEndogene))
				{
					if (Props.xenotypeDef != null && (caster.genes.Xenotype is not DevXenotypeDef hybrid || !hybrid.isHybrid))
					{
						ReimplanterUtility.SetXenotypeDirect(null, caster, Props.xenotypeDef);
					}
					ReimplanterUtility.UpdateXenogermReplication_WithComa(caster);
					ReimplanterUtility.ExtractXenogerm(victim);
					ReimplanterUtility.FleckAndLetter(victim, caster);
					//Gene.SetXenotypes(caster.genes.Xenotype, victim.genes.Xenotype);
				}
				else
                {
					Messages.Message("WVC_XaG_HybridImplanterFail".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (Gene == null)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_Implanter_MissingGeneMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return ReimplanterUtility.ImplanterValidation(parent.pawn, target, throwMessages) && base.Valid(target, throwMessages);
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			if (GeneUtility.PawnWouldDieFromReimplanting(target.Pawn))
			{
				return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(target.Pawn.Named("PAWN")) + "\n\n" + "WVC_XaG_HybridImplanter_Warning".Translate(parent.pawn).Colorize(ColorLibrary.Gold), confirmAction, destructive: true);
			}
			return Dialog_MessageBox.CreateConfirmation("WVC_XaG_HybridImplanter_Warning".Translate(parent.pawn).Colorize(ColorLibrary.Gold), confirmAction, destructive: true);
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

	}

}
