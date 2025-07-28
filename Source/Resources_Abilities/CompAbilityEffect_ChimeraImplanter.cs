using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompAbilityEffect_ChimeraImplanter : CompAbilityEffect_ChimeraDependant
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				if (TryReimplant(parent.pawn, pawn))
				{
					ReimplanterUtility.FleckAndLetter(parent.pawn, pawn);
				}
			}
		}

		private bool TryReimplant(Pawn caster, Pawn target)
        {
			if (!target.IsHuman() || !caster.IsHuman())
            {
				return false;
            }
			int genesCount = 0;
			foreach (Gene gene in caster.genes.Endogenes)
            {
				if (gene == ChimeraGene || gene.def.prerequisite == ChimeraGene.def)
                {
					target.genes.AddGene(gene.def, false);
					genesCount++;
				}
			}
            IntRange durationIntervalRange = new(40000 * genesCount, 50000 * genesCount);
            ReimplanterUtility.XenogermReplicating_WithCustomDuration(caster, durationIntervalRange);
            ReimplanterUtility.XenogermReplicating_WithCustomDuration(target, durationIntervalRange);
			//ReimplanterUtility.ExtractXenogerm(caster);
			return true;
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (!base.Valid(target, throwMessages) || !ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages))
            {
                return false;
            }
            Pawn victim = target.Pawn;
            if (victim?.genes?.GetFirstGeneOfType<Gene_Chimera>() != null)
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_GeneChimera_TargetIsChimera".Translate(), victim, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return true;
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
