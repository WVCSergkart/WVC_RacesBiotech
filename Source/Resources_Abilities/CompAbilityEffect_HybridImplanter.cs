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
			Pawn victim = target.Pawn;
			if (victim != null)
			{
                Pawn caster = parent.pawn;
                if (SubXenotypeUtility.TrySetHybridXenotype(caster, victim, Gene, !caster.genes.IsXenogene(Gene)))
				{
					ReimplanterUtility.UpdateXenogermReplication_WithComa(caster);
					ReimplanterUtility.ExtractXenogerm(victim);
					ReimplanterUtility.FleckAndLetter(caster, victim);
				}
				else
                {
					Messages.Message("WVC_XaG_HybridImplanterFail".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			return ReimplanterUtility.ImplanterValidation(parent.pawn, target, throwMessages) && base.Valid(target, throwMessages);
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			return null;
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

	}

}
