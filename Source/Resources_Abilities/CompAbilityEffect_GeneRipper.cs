using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class CompAbilityEffect_GeneRipper : CompAbilityEffect_ChimeraDependant
	{

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn victim = target.Pawn;
			if (victim != null)
			{
                if (GetNotMissingParts(victim).TryRandomElement(out BodyPartRecord partRecord) && ChimeraGene.TryGetGene(victim, out GeneDef result))
                {
                    if (Props.opinionThoughtDefToGiveTarget != null)
                    {
                        victim.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.opinionThoughtDefToGiveTarget, parent.pawn);
                    }
                    if (Props.thoughtDefToGiveTarget != null)
                    {
                        victim.needs?.mood?.thoughts?.memories?.TryGainMemory(Props.thoughtDefToGiveTarget);
                    }
                    if (result.passOnDirectly && !XaG_GeneUtility.ConflictWith(result, parent.pawn.genes.GenesListForReading))
                    {
                        parent.pawn.genes.AddGene(result, true);
                        ReimplanterUtility.PostImplantDebug(parent.pawn);
                    }
                    Messages.Message("WVC_XaG_GeneRipper_GeneCopied".Translate(parent.pawn.NameShortColored, result.label), parent.pawn, MessageTypeDefOf.NeutralEvent, historical: false);
                    victim.genes.RemoveGene(victim.genes.GetGene(result));
                    ReimplanterUtility.PostImplantDebug(victim);
                    ReimplanterUtility.TryFixPawnXenotype_Beta(victim);
                    ReimplanterUtility.TrySetSkinAndHairGenes(victim);
                    SoundDefOf.Execute_Cut.PlayOneShot(victim);
                    if (victim.health.CanBleed)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, victim);
                        hediff.Severity = 0.2f * partRecord.coverage;
                        victim.health.AddHediff(hediff);
                        GeneFeaturesUtility.TrySpawnBloodFilth(victim, new(2, 3));
                    }
                    victim.health.AddHediff(HediffDefOf.MissingBodyPart, partRecord);
                }
            }
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (!base.Valid(target, throwMessages))
            {
                return false;
            }
            if (!ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, false, false))
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_GeneralWrongTarget".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            Pawn victim = target.Pawn;
            if (GetNotMissingParts(victim).Count <= 0)
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_GeneRipper_TargetNoParts".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (victim.genes.GenesListForReading.Where((gene) => !ChimeraGene.AllGenes.Contains(gene.def)).ToList().Count <= 0)
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_GeneChimera_TargetNoGenes".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return true;
        }

        private static List<BodyPartRecord> GetNotMissingParts(Pawn victim)
        {
            return victim.health.hediffSet.GetNotMissingParts().Where((part) => !part.def.conceptual && part.def.destroyableByDamage).ToList();
        }

        //public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
        //{
        //	if (target.Pawn.health.WouldDieAfterAddingHediff(HediffDefOf.MissingBodyPart, , 1f))
        //	{
        //		return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
        //	}
        //	return null;
        //}

        public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

	}

}
