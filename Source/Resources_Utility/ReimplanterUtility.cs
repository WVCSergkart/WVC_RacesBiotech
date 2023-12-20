using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class ReimplanterUtility
	{

		public static void ReimplantEndogerm(Pawn caster, Pawn recipient)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
			ReimplanterUtility.ReimplantGenesBase(caster, recipient);
			GeneUtility.ExtractXenogerm(caster);
		}

		// ===============================================================

		public static void ReimplantXenogenesFromXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			pawn.genes.SetXenotypeDirect(xenotypeDef);
			// pawn.genes.xenotypeName = null;
			// pawn.genes.iconDef = null;
			// pawn.genes.cachedHasCustomXenotype = null;
			// pawn.genes.cachedCustomXenotype = null;
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
			pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			GeneUtility.UpdateXenogermReplication(pawn);
		}

		public static void SaveReimplantXenogenesFromXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			if (pawn.genes != null && pawn.genes.Xenogenes.Count <= 0)
			{
				ReimplantXenogenesFromXenotype(pawn, xenotypeDef);
			}
		}

		public static void ReimplantGenesBase(Pawn caster, Pawn recipient)
		{
			// recipient.genes.SetXenotype(caster.genes.Xenotype);
			recipient.genes.SetXenotypeDirect(caster.genes.Xenotype);
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
			GeneUtility.UpdateXenogermReplication(recipient);
		}

		public static List<GeneDef> GenesNonCandidatesForSerums()
		{
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.nonCandidatesForSerums);
			}
			return list;
		}

		public static List<GeneDef> GenesPerfectCandidatesForSerums()
		{
			List<GeneDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.perfectCandidatesForSerums);
			}
			return list;
		}

		public static bool DelayedReimplanterIsActive(Pawn pawn)
		{
			if (pawn.health != null && pawn.health.hediffSet != null)
			{
				List<HediffDef> hediffDefs = new();
				foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				{
					hediffDefs.AddRange(item.blackListedHediffDefForReimplanter);
				}
				for (int i = 0; i < hediffDefs.Count; i++)
				{
					if (pawn.health.hediffSet.HasHediff(hediffDefs[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		// =============================== Setter ===============================

		public static void SetXenotype_DoubleXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty() && Rand.Value < xenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) && xenotypeDef.doubleXenotypeChances.TryRandomElementByWeight((XenotypeChance x) => x.chance, out var result))
			{
				SetXenotype(pawn, result.xenotype);
			}
			SetXenotype(pawn, xenotypeDef);
		}

		public static void SetXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			// remove all genes
			Pawn_GeneTracker genes = pawn.genes;
			for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
			{
				pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
			}
			if (xenotypeDef.inheritable)
			{
				for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
				{
					pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
				}
			}
			// Add genes
			pawn.genes?.SetXenotypeDirect(xenotypeDef);
			bool xenotypeHasSkinColor = false;
			bool xenotypeHasHairColor = false;
			List<GeneDef> xenotypeGenes = xenotypeDef.genes;
			for (int i = 0; i < xenotypeGenes.Count; i++)
			{
				pawn.genes?.AddGene(xenotypeGenes[i], !xenotypeDef.inheritable);
				if (xenotypeGenes[i].skinColorBase != null || xenotypeGenes[i].skinColorOverride != null)
				{
					xenotypeHasSkinColor = true;
				}
				if (xenotypeGenes[i].hairColorOverride != null)
				{
					xenotypeHasHairColor = true;
				}
			}
			if (xenotypeDef.inheritable && (!xenotypeHasSkinColor || xenotypeDef == XenotypeDefOf.Baseliner))
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Skin_SheerWhite, !xenotypeDef.inheritable);
			}
			if (xenotypeDef.inheritable && (!xenotypeHasHairColor || xenotypeDef == XenotypeDefOf.Baseliner))
			{
				pawn.genes?.AddGene(WVC_GenesDefOf.Hair_SnowWhite, !xenotypeDef.inheritable);
			}
		}

	}
}
