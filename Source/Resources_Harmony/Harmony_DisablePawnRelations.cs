using HarmonyLib;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(PawnGenerator), "GeneratePawnRelations")]
		public static class Patch_PawnGenerator_GeneratePawnRelations
		{
			[HarmonyPrefix]
			public static bool DisablePawnRelations(Pawn pawn)
			{
				if (!XaG_GeneUtility.HasAnyActiveGene(new() { WVC_GenesDefOf.WVC_FemaleOnly, WVC_GenesDefOf.WVC_MaleOnly }, pawn))
				{
					return true;
				}
				return false;
			}
		}

	}

}
