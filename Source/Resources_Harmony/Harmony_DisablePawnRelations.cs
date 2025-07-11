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
				//if (!XaG_GeneUtility.HasAnyActiveGene(new() { MainDefOf.WVC_FemaleOnly, MainDefOf.WVC_MaleOnly }, pawn))
				//{
				//	return true;
				//}
				if (pawn.genes?.GetFirstGeneOfType<Gene_Gender>() == null)
				{
					return true;
				}
				return false;
			}
		}

	}

}
