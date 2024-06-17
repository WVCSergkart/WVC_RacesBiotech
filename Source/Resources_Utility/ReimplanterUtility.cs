using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class ReimplanterUtility
	{

		public static bool TryReimplant(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true)
		{
			if (!recipient.IsHuman() || !caster.IsHuman())
			{
				return false;
			}
			QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
			ReimplanterUtility.ReimplantGenesHybrid(caster, recipient, endogenes, xenogenes);
			ExtractXenogerm(caster);
			return true;
		}

		[Obsolete]
		public static void Reimplanter(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true)
		{
			TryReimplant(caster, recipient, endogenes, xenogenes);
		}

		public static void ExtractXenogerm(Pawn pawn, int overrideDurationTicks = -1)
		{
			pawn.health.AddHediff(HediffDefOf.XenogermLossShock);
			if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
			{
				SetXenotype(pawn, XenotypeDefOf.Baseliner);
			}
			Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
			if (overrideDurationTicks > 0)
			{
				hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
			}
			pawn.health.AddHediff(hediff);
		}

		public static void SetXenotypeDirect(Pawn caster, Pawn recipient, XenotypeDef xenotypeDef = null, bool changeXenotype = true)
		{
			if (changeXenotype)
			{
				recipient.genes?.SetXenotypeDirect(caster != null ? caster.genes.Xenotype : xenotypeDef);
				recipient.genes.xenotypeName = caster?.genes.xenotypeName;
				recipient.genes.iconDef = caster?.genes.iconDef;
			}
		}

		public static void UnknownXenotype(Pawn pawn, string xenotypeName = null, XenotypeIconDef xenotypeIconDef = null)
		{
			// pawn.genes.xenotypeName = "WVC_XaG_SubXenotypeUnknownXenotypeError".Translate();
			// pawn.genes.iconDef = WVC_GenesDefOf.WVC_XenoRandomKindc;
			pawn.genes?.SetXenotypeDirect(XenotypeDefOf.Baseliner);
			pawn.genes.xenotypeName = xenotypeName ?? GeneUtility.GenerateXenotypeNameFromGenes(XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading));
			pawn.genes.iconDef = xenotypeIconDef ?? DefDatabase<XenotypeIconDef>.AllDefsListForReading.RandomElement();
		}

		public static void GiveReimplantJob(Pawn pawn, Pawn targPawn, JobDef absorbJob)
		{
			pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(absorbJob, targPawn), JobTag.Misc);
			if (targPawn.HomeFaction != null && !targPawn.HomeFaction.Hidden && targPawn.HomeFaction != pawn.Faction && !targPawn.HomeFaction.HostileTo(Faction.OfPlayer))
			{
				Messages.Message("MessageAbsorbingXenogermWillAngerFaction".Translate(targPawn.HomeFaction, targPawn.Named("PAWN")), pawn, MessageTypeDefOf.CautionInput, historical: false);
			}
		}

		public static bool CanAbsorbGenogerm(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			// if (!pawn.genes.HasGene(GeneDefOf.XenogermReimplanter))
			// {
				// return false;
			// }
			// if (!MiscUtility.HasReimplanterAbility(pawn))
			// {
				// return false;
			// }
			// if (!XaG_GeneUtility.HasActiveGene(gene.def, pawn))
			// {
				// return false;
			// }
			if (pawn.IsPrisonerOfColony && pawn.guest.PrisonerIsSecure)
			{
				return true;
			}
			if (!pawn.Downed)
			{
				return false;
			}
			if (!pawn.genes.GenesListForReading.Any())
			{
				return false;
			}
			return true;
		}

		// ===============================================================

		[Obsolete]
		public static void ReimplantXenogenesFromXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			// pawn.genes.SetXenotypeDirect(xenotypeDef);
			// pawn.genes.xenotypeName = null;
			// pawn.genes.iconDef = null;
			// pawn.genes.cachedHasCustomXenotype = null;
			// pawn.genes.cachedCustomXenotype = null;
			SetXenotypeDirect(null, pawn, xenotypeDef);
			Pawn_GeneTracker pawnGenes = pawn.genes;
			if (pawnGenes != null && pawnGenes.Xenogenes.Count > 0)
			{
				foreach (Gene item in pawn.genes?.Xenogenes)
				{
					pawn.genes?.RemoveGene(item);
				}
			}
			foreach (GeneDef xenogene in xenotypeDef.genes)
			{
				pawn.genes.AddGene(xenogene, xenogene: true);
			}
			if (!xenotypeDef.soundDefOnImplant.NullOrUndefined())
			{
				xenotypeDef.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(pawn));
			}
		}

		public static void SaveReimplantXenogenesFromXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			if (pawn.genes != null && pawn.genes.Xenogenes.NullOrEmpty())
			{
				SetXenotype(pawn, xenotypeDef);
				pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
				GeneUtility.UpdateXenogermReplication(pawn);
			}
		}

		public static void ReimplantGenesHybrid(Pawn caster, Pawn recipient, bool endogenes = true, bool xenogenes = true)
		{
			Pawn_GeneTracker recipientGenes = recipient.genes;
			if (recipientGenes.Xenogenes.NullOrEmpty() || xenogenes)
			{
				SetXenotypeDirect(caster, recipient);
			}
			if (xenogenes && !recipientGenes.Xenogenes.NullOrEmpty())
			{
				// foreach (Gene item in recipientGenes.Xenogenes.ToList())
				// {
					// recipient.genes?.RemoveGene(item);
				// }
				recipientGenes.Xenogenes.RemoveAllGenes();
			}
			if (endogenes && !recipientGenes.Endogenes.NullOrEmpty())
			{
				// foreach (Gene item in recipientGenes.Endogenes.ToList())
				// {
					// recipient.genes?.RemoveGene(item);
				// }
				recipientGenes.Endogenes.RemoveAllGenes();
			}
			if (endogenes)
			{
				foreach (Gene endogene in caster.genes.Endogenes)
				{
					recipient.genes.AddGene(endogene.def, xenogene: false);
				}
			}
			if (xenogenes)
			{
				foreach (Gene xenogene in caster.genes.Xenogenes)
				{
					recipient.genes.AddGene(xenogene.def, xenogene: true);
				}
			}
			if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
			{
				caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
			}
			recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(recipient);
		}

		// =============================== New ===============================

		[Obsolete]
		public static void ReimplantGenesDouble_Shapeshifter(Pawn pawn, XenotypeDef xenotypeDef, bool xenogenes = true, bool doubleXenotypes = true)
		{
			if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result) && doubleXenotypes)
			{
				ReimplantGenes_Shapeshifter(pawn, result.xenotype, false);
			}
			ReimplantGenes_Shapeshifter(pawn, xenotypeDef, xenogenes);
		}

		[Obsolete]
		public static void ReimplantGenes_Shapeshifter(Pawn pawn, XenotypeDef xenotypeDef, bool xenogenes = true)
		{
			Pawn_GeneTracker recipientGenes = pawn.genes;
			if (recipientGenes.Xenogenes.NullOrEmpty() || xenogenes)
			{
				SetXenotypeDirect(null, pawn, xenotypeDef, true);
			}
			if (xenogenes || !xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				recipientGenes.Xenogenes.RemoveAllGenes();
			}
			if (xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				recipientGenes.Endogenes.RemoveAllGenes();
			}
			foreach (GeneDef geneDef in xenotypeDef.genes)
			{
				pawn.genes.AddGene(geneDef, !xenotypeDef.inheritable);
			}
			TrySetSkinAndHairGenes(pawn);
		}

		public static void TrySetSkinAndHairGenes(Pawn pawn)
		{
			Pawn_GeneTracker recipientGenes = pawn.genes;
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			foreach (Gene gene in recipientGenes.GenesListForReading)
			{
				if (gene.def.skinColorBase != null || gene.def.skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (gene.def.hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (!xenotypeHasSkinColor)
			{
				recipientGenes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, false);
			}
			if (!xenotypeHasHairColor)
			{
				recipientGenes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, false);
			}
		}

		// =============================== Setter ===============================

		public static void SetXenotype_DoubleXenotype(Pawn pawn, XenotypeDef xenotypeDef, bool changeXenotype = true)
		{
			if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result))
			{
				SetXenotype(pawn, result.xenotype, changeXenotype);
			}
			SetXenotype(pawn, xenotypeDef, changeXenotype);
		}

		public static void SetXenotype(Pawn pawn, XenotypeDef xenotypeDef, bool changeXenotype = true)
		{
			// Remove genes
			Pawn_GeneTracker genes = pawn.genes;
			genes.Xenogenes.RemoveAllGenes();
			if (xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner)
			{
				genes.Endogenes.RemoveAllGenes();
			}
			// Add genes
			SetXenotypeDirect(null, pawn, xenotypeDef, changeXenotype);
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			List<GeneDef> xenotypeGenes = xenotypeDef.genes;
			for (int i = 0; i < xenotypeGenes.Count; i++)
			{
				genes?.AddGene(xenotypeGenes[i], !xenotypeDef.inheritable);
				if (xenotypeGenes[i].skinColorBase != null || xenotypeGenes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (xenotypeGenes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if ((xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner) && !xenotypeHasSkinColor)
			{
				genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, false);
			}
			if ((xenotypeDef.inheritable || xenotypeDef == XenotypeDefOf.Baseliner) && !xenotypeHasHairColor)
			{
				genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, false);
			}
		}

	}
}
