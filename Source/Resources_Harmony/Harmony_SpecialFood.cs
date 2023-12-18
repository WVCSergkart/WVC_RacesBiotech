using HarmonyLib;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(FoodUtility), "FoodOptimality")]
		public static class Patch_FoodUtility_FoodOptimality
		{

			[HarmonyPrefix]
			public static bool Prefix(ref float __result, ref Pawn eater, ref Thing foodSource)
			{
				FoodExtension_GeneFood foodExtension = foodSource.def.GetModExtension<FoodExtension_GeneFood>();
				if (foodExtension != null)
				{
					if (!foodExtension.geneDefs.NullOrEmpty() && XaG_GeneUtility.HasAnyActiveGene(foodExtension.geneDefs, eater))
					{
						__result = 999999f;
					}
					else
					{
						__result = -999999f;
					}
					return false;
				}
				return true;
			}
		}

	}

}
