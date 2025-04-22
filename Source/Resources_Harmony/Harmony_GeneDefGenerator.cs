using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
// using static RimWorld.BaseGen.SymbolStack;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(GeneDefGenerator), "ImpliedGeneDefs")]
		public static class Patch_GeneDefGenerator_ImpliedGeneDefs
		{
			[HarmonyPostfix]
			public static IEnumerable<GeneDef> Postfix(IEnumerable<GeneDef> values)
			{
				List<GeneDef> geneDefList = values.ToList();
				GeneratorUtility.Aptitudes(geneDefList);
				GeneratorUtility.HybridForcerGenes(geneDefList);
				//GeneratorUtility.Spawners(geneDefList);
				//GeneratorUtility.AutoColorGenes(geneDefList);
				GeneratorUtility.GauranlenTreeModeDef();
				return geneDefList;
			}

        }

	}

}
