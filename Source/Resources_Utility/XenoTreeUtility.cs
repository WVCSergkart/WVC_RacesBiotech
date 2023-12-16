using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class XenoTreeUtility
	{

		public static bool XenoTree_CanSpawn(XenotypeDef xenotypeDef, Thing parent)
		{
			if (xenotypeDef.genes.NullOrEmpty())
			{
				return true;
			}
			if (XenoTree_ToxResCheck(xenotypeDef, parent))
			{
				return true;
			}
			return false;
		}

		public static bool XenoTree_ToxResCheck(XenotypeDef xenotypeDef, Thing parent)
		{
			if (!XenotypeHasToxResistance(xenotypeDef) || parent.Position.IsPolluted(parent.Map))
			{
				return true;
			}
			return false;
		}

		// public static bool XenoTree_ColdResCheck(XenotypeDef xenotypeDef, Thing parent)
		// {
			// if (!XenotypeHasColdResistance(xenotypeDef) || parent.Map.mapTemperature.SeasonalTemp <= -20f)
			// {
				// return true;
			// }
			// return false;
		// }

		// public static bool XenoTree_HeatResCheck(XenotypeDef xenotypeDef, Thing parent)
		// {
			// if (!XenotypeHasHeatResistance(xenotypeDef) || parent.Map.mapTemperature.SeasonalTemp >= 20f)
			// {
				// return true;
			// }
			// return false;
		// }

		// =============================== ToxResistance ===============================

		// Tox
		public static bool XenotypeHasToxResistance(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			if (genes.NullOrEmpty())
			{
				return false;
			}
			float toxRes = 0f;
			float toxEnvRes = 0f;
			List<XenotypeDef> xenotypes = GetXenotypeAndDoubleXenotypes(xenotypeDef);
			foreach (XenotypeDef xenotype in xenotypes)
			{
				foreach (GeneDef item in xenotype.genes)
				{
					if (!item.statOffsets.NullOrEmpty())
					{
						foreach (StatModifier statModifier in item.statOffsets)
						{
							if (statModifier.stat == StatDefOf.ToxicResistance)
							{
								toxRes += statModifier.value;
							}
							if (statModifier.stat == StatDefOf.ToxicEnvironmentResistance)
							{
								toxEnvRes += statModifier.value;
							}
						}
					}
				}
			}
			if (toxRes >= 1f || toxEnvRes >= 1f)
			{
				return true;
			}
			return false;
		}

		// Heat
		public static bool XenotypeHasHeatResistance(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			if (genes.NullOrEmpty())
			{
				return false;
			}
			float temperatureMax = 0f;
			foreach (GeneDef item in xenotypeDef.genes)
			{
				if (!item.statOffsets.NullOrEmpty())
				{
					foreach (StatModifier statModifier in item.statOffsets)
					{
						if (statModifier.stat == StatDefOf.ComfyTemperatureMax)
						{
							temperatureMax += statModifier.value;
						}
					}
				}
			}
			if (temperatureMax >= 20f)
			{
				return true;
			}
			return false;
		}

		// Cold
		public static bool XenotypeHasColdResistance(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			if (genes.NullOrEmpty())
			{
				return false;
			}
			float temperatureMin = 0f;
			foreach (GeneDef item in xenotypeDef.genes)
			{
				if (!item.statOffsets.NullOrEmpty())
				{
					foreach (StatModifier statModifier in item.statOffsets)
					{
						if (statModifier.stat == StatDefOf.ComfyTemperatureMin)
						{
							temperatureMin += statModifier.value;
						}
					}
				}
			}
			if (temperatureMin <= -20f)
			{
				return true;
			}
			return false;
		}

		// =============================== Other ===============================

		public static bool XenotypeIsFurskin(XenotypeDef xenotypeDef)
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
					if (item.graphicData != null && item.graphicData.fur != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool XenotypeIsArchite(XenotypeDef xenotypeDef)
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
					if (item.biostatArc > 0)
					{
						return false;
					}
				}
			}
			return false;
		}

		public static bool XenotypeIsBloodfeeder(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			if (genes.NullOrEmpty())
			{
				return false;
			}
			List<XenotypeDef> xenotypes = GetXenotypeAndDoubleXenotypes(xenotypeDef);
			foreach (XenotypeDef xenotype in xenotypes)
			{
				GeneDef geneDef = GetFirstGeneOfType(xenotype.genes, typeof(Gene_Hemogen));
				if (geneDef != null)
				{
					return true;
				}
			}
			return false;
		}

		public static bool XenotypeIsUndead(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			if (genes.NullOrEmpty())
			{
				return false;
			}
			List<XenotypeDef> xenotypes = GetXenotypeAndDoubleXenotypes(xenotypeDef);
			foreach (XenotypeDef xenotype in xenotypes)
			{
				GeneDef geneDef = GetFirstGeneOfType(xenotype.genes, typeof(Gene_Undead));
				if (geneDef != null)
				{
					return true;
				}
			}
			return false;
		}

		// =============================== Getter ===============================

		public static int GetAllGenesCount(XenotypeDef xenotypeDef)
		{
			int genesCount = 0;
			List<XenotypeDef> xenotypes = GetXenotypeAndDoubleXenotypes(xenotypeDef);
			foreach (XenotypeDef xenotype in xenotypes)
			{
				genesCount += xenotype.genes.Count;
			}
			return genesCount;
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

		public static GeneDef GetFirstGeneOfType(List<GeneDef> genes, Type type)
		{
			for (int i = 0; i < genes.Count; i++)
			{
				if (genes[i].geneClass == type)
				{
					return genes[i];
				}
			}
			return null;
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
