using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class XenotypeUtility
	{

		public static bool XenoTree_CanSpawn(XenotypeDef xenotypeDef, Thing parent)
		{
			if (xenotypeDef.genes.NullOrEmpty())
			{
				return false;
			}
			if (!XenotypeHasToxResistance(xenotypeDef) || parent.Position.IsPolluted(parent.Map))
			{
				return true;
			}
			return false;
		}

		// public static bool XenoTree_CanSpawnArchiteXenos(XenotypeDef xenotypeDef)
		// {
			// if (XenotypeIsArchite(xenotypeDef) && MechanitorUtility.AnyMechanitorInPlayerFaction())
			// {
				// return true;
			// }
			// return false;
		// }

		// Basic Checks

		public static bool XenotypeHasToxResistance(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			if (genes.NullOrEmpty())
			{
				return false;
			}
			List<XenotypeDef> xenotypes = GetXenotypeAndDoubleXenotypes(xenotypeDef);
			foreach (XenotypeDef xenotype in xenotypes)
			{
				foreach (GeneDef item in xenotype.genes)
				{
					if (!item.exclusionTags.NullOrEmpty() && (item.exclusionTags.Contains("ToxicEnvironmentResistance") || item.exclusionTags.Contains("ToxResistance")))
					{
						return true;
					}
					if (item.immuneToToxGasExposure)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static List<XenotypeDef> GetXenotypeAndDoubleXenotypes(XenotypeDef xenotypeDef)
		{
			List<XenotypeDef> xenotypes = new();
			xenotypes.Add(xenotypeDef);
			if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty())
			{
				foreach (XenotypeChance item in xenotypeDef.doubleXenotypeChances)
				{
					xenotypes.Add(item.xenotype);
				}
			}
			return xenotypes;
		}

		// public static bool XenotypeHasAnyDiseaseResistance(XenotypeDef xenotypeDef)
		// {
			// List<GeneDef> genes = xenotypeDef.genes;
			// if (genes.NullOrEmpty())
			// {
				// return false;
			// }
			// foreach (GeneDef item in genes)
			// {
				// if (!item.makeImmuneTo.NullOrEmpty())
				// {
					// return true;
				// }
				// if (!item.hediffGiversCannotGive.NullOrEmpty())
				// {
					// return true;
				// }
			// }
			// return false;
		// }

		// public static bool XenotypeIsArchite(XenotypeDef xenotypeDef)
		// {
			// List<GeneDef> genes = xenotypeDef.genes;
			// if (genes.NullOrEmpty())
			// {
				// return false;
			// }
			// int arc = 0;
			// foreach (GeneDef item in genes)
			// {
				// arc += item.biostatArc;
			// }
			// if (arc > 0)
			// {
				// return true;
			// }
			// return false;
		// }

		// Setter

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
