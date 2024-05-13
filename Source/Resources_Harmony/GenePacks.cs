using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace WVC_XenotypesAndGenes
{

	[HarmonyPatch(typeof(Genepack))]
	[HarmonyPatch("PostMake")]
	public static class Patch_Genepack_PostMake
	{

		[HarmonyPostfix]
		public static void ChangeGenesInPack(Genepack __instance, ref GeneSet ___geneSet)
		{
			CompProperties_Genepack xag_genepack = __instance?.TryGetComp<CompGenepack>()?.Props;
			if (xag_genepack != null)
			{
				ThingDef thingDef = __instance.def;
				GeneSet newGeneSet = new();
				XaG_CountWithChance geneCount = xag_genepack.genesCountProbabilities.RandomElementByWeight((XaG_CountWithChance x) => x.chance);
				XaG_GeneUtility.SetGenesInPack(geneCount, newGeneSet);
				newGeneSet.SortGenes();
				XaG_GeneUtility.GenerateName(newGeneSet, xag_genepack.genepackNamer);
				if (!newGeneSet.Empty)
				{
					___geneSet = newGeneSet;
				}
				else
				{
					Log.Warning(thingDef.LabelCap + " generated with null geneSet. Vanilla geneSet will be used instead.");
				}
				if (xag_genepack.styleDef != null)
				{
					__instance.SetStyleDef(xag_genepack.styleDef);
				}
				__instance.def = ThingDefOf.Genepack;
			}
		}

		// [HarmonyPostfix]
		// public static void ChangeGenesInPack(Genepack __instance, ref GeneSet ___geneSet)
		// {
			// ThingDef thingDef = __instance.def;
			// if (thingDef.IsXenoGenesDef())
			// {
				// GeneSet newGeneSet = new();
				// GeneExtension_General geneExtension = thingDef?.GetModExtension<GeneExtension_General>();
				// if (geneExtension != null)
				// {
					// XaG_CountWithChance geneCount = geneExtension.genesCountProbabilities.RandomElementByWeight((XaG_CountWithChance x) => x.chance);
					// MiscUtility.SetGenesInPack(geneCount, newGeneSet);
					// newGeneSet.SortGenes();
					// MiscUtility.GenerateName(newGeneSet, geneExtension.genepackNamer);
				// }
				// if (!newGeneSet.Empty)
				// {
					// ___geneSet = newGeneSet;
				// }
				// else
				// {
					// Log.Warning(thingDef.LabelCap + " generated with null geneSet. Vanilla geneSet will be used instead.");
				// }
			// }
		// }

	}

}
