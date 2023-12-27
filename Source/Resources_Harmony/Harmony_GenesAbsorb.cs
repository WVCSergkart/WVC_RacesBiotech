using HarmonyLib;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		// [HarmonyPatch(typeof(SanguophageUtility), "ShouldBeDeathrestingOrInComaInsteadOfDead")]
		// public static class Patch_SanguophageUtility_ShouldBeDeathrestingOrInComaInsteadOfDead
		// {

			// [HarmonyPrefix]
			// public static bool Prefix(ref bool __result, ref Pawn pawn)
			// {
				// if (UndeadUtility.IsUndead(pawn) && pawn.genes.GetFirstGeneOfType<Gene_Undead>().UndeadCanResurrect)
				// {
					// if (!pawn.health.ShouldBeDead())
					// {
						// return true;
					// }
					// __result = true;
					// UndeadUtility.ShouldUndeadRegenComaOrDeathrest(__result, pawn);
					// return false;
				// }
				// return true;
			// }
		// }

	}

}
