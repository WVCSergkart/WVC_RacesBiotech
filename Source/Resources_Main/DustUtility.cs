using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public static class DustUtility
	{

		public static void OffsetDust(Pawn pawn, float offset)
		{
			if (!ModsConfig.BiotechActive)
			{
				return;
			}
			Gene_Dust gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			if (gene_Hemogen != null)
			{
				gene_Hemogen.Value += offset;
			}
		}

		public static bool PawnInPronePosition(Pawn pawn)
		{
			if (pawn.Downed || pawn.Deathresting || RestUtility.InBed(pawn) || !RestUtility.Awake(pawn))
			{
				return true;
			}
			return false;
		}

		public static void ReimplantGenes(Pawn caster, Pawn recipient)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			// QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
			recipient.genes.SetXenotype(caster.genes.Xenotype);
			recipient.genes.xenotypeName = caster.genes.xenotypeName;
			recipient.genes.iconDef = caster.genes.iconDef;
			Pawn_GeneTracker recipientGenes = recipient.genes;
			if (recipientGenes != null && recipientGenes.GenesListForReading.Count > 0)
			{
				foreach (Gene item in recipient.genes?.GenesListForReading)
				{
					recipient.genes?.RemoveGene(item);
				}
			}
			foreach (Gene endogene in caster.genes.Endogenes)
			{
				recipient.genes.AddGene(endogene.def, xenogene: false);
			}
			foreach (Gene xenogene in caster.genes.Xenogenes)
			{
				recipient.genes.AddGene(xenogene.def, xenogene: true);
			}
			if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
			{
				caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
			}
			recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
			// GeneUtility.ExtractXenogerm(caster);
			GeneUtility.UpdateXenogermReplication(recipient);
			if (PawnUtility.ShouldSendNotificationAbout(caster) || PawnUtility.ShouldSendNotificationAbout(recipient))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(caster.Named("CASTER"), recipient.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(caster, recipient));
			}
		}

	}
}
