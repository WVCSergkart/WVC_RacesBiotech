using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class XenoTreeUtility
	{

		public static bool IsXenoTree(this Thing parent)
		{
			if (parent.def.plant != null && parent.TryGetComp<CompXenoTree>() != null)
			{
				return true;
			}
			return false;
		}

		public static bool IsResurgentTree(this Thing parent)
		{
			if (parent.def.plant != null && parent.TryGetComp<CompWalkingCorpsesSpawner>() != null)
			{
				return true;
			}
			return false;
		}

		// =================================

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
				GeneDef geneDef = XaG_GeneUtility.GetFirstGeneDefOfType(xenotype.genes, typeof(Gene_Hemogen));
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
				GeneDef geneDef = XaG_GeneUtility.GetFirstGeneDefOfType(xenotype.genes, typeof(Gene_Undead));
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

	}
}
