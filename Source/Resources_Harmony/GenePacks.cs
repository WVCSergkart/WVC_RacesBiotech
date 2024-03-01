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
			ThingDef thingDef = __instance.def;
			if (thingDef.IsFromXenoGenes())
			{
				GeneSet newGeneSet = new();
				int? seed = null;
				if (seed.HasValue)
				{
					Rand.PushState(seed.Value);
				}
				GeneExtension_General geneExtension = thingDef?.GetModExtension<GeneExtension_General>();
				if (geneExtension != null)
				{
					XaG_CountWithChance geneCount = geneExtension.genesCountProbabilities.RandomElementByWeight((XaG_CountWithChance x) => x.chance);
					SetGenesInPack(geneCount, newGeneSet);
					GenerateName(newGeneSet, geneExtension.genepackNamer);
				}
				if (seed.HasValue)
				{
					Rand.PopState();
				}
				if (!newGeneSet.Empty)
				{
					newGeneSet.SortGenes();
					___geneSet = newGeneSet;
				}
				else
				{
					Log.Warning(thingDef.LabelCap + " generated with null geneSet. Vanilla geneSet will be used instead.");
				}
			}
		}

		private static void GenerateName(GeneSet geneSet, RulePackDef rule)
		{
			if (rule == null)
			{
				return;
			}
			if (geneSet.GenesListForReading.Any())
			{
				GrammarRequest request = default;
				request.Includes.Add(rule);
				request.Rules.Add(new Rule_String("geneWord", geneSet.GenesListForReading[0].LabelShortAdj));
				request.Rules.Add(new Rule_String("geneCountMinusOne", (geneSet.GenesListForReading.Count - 1).ToString()));
				request.Constants.Add("geneCount", geneSet.GenesListForReading.Count.ToString());
				Type typeFromHandle = typeof(GeneSet);
				FieldInfo field = typeFromHandle.GetField("name", BindingFlags.Instance | BindingFlags.NonPublic);
				field.SetValue(geneSet, GrammarResolver.Resolve("r_name", request, null, forceLog: false, null, null, null, capitalizeFirstSentence: false));
			}
		}

		private static void SetGenesInPack(XaG_CountWithChance geneCount, GeneSet geneSet)
		{
			for (int j = 0; j < geneCount.genesCount; j++)
			{
				if (DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef x) => x.biostatArc == 0 && x.IsFromXenoGenes() && CanAddGeneDuringGeneration(x, geneSet)).TryRandomElementByWeight((GeneDef x) => x.selectionWeight, out var result))
				{
					geneSet.AddGene(result);
				}
			}
			for (int i = 0; i < geneCount.architeCount; i++)
			{
				if (DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef x) => x.biostatArc != 0 && x.IsFromXenoGenes() && CanAddGeneDuringGeneration(x, geneSet)).TryRandomElementByWeight((GeneDef x) => x.selectionWeight, out var result))
				{
					geneSet.AddGene(result);
				}
			}
		}

		private static bool CanAddGeneDuringGeneration(GeneDef gene, GeneSet geneSet)
		{
			List<GeneDef> genes = geneSet.GenesListForReading;
			if (!ModsConfig.BiotechActive)
			{
				return false;
			}
			if (!gene.canGenerateInGeneSet || gene.selectionWeight <= 0f)
			{
				return false;
			}
			if (genes.Contains(gene))
			{
				return false;
			}
			if (genes.Count > 0 && !GeneTuning.BiostatRange.Includes(gene.biostatMet + geneSet.MetabolismTotal))
			{
				return false;
			}
			// if (gene.prerequisite != null && !genes.Contains(gene.prerequisite))
			// {
				// return false;
			// }
			for (int i = 0; i < genes.Count; i++)
			{
				if (gene.ConflictsWith(genes[i]))
				{
					return false;
				}
			}
			return true;
		}
	}

}
