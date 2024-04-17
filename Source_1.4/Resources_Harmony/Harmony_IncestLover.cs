using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(RelationsUtility), "Incestuous")]
		public static class Patch_RelationsUtility_Incestuous
		{

			[HarmonyPrefix]
			public static bool Prefix(ref bool __result, ref Pawn one)
			{
				if (one?.genes?.GetFirstGeneOfType<Gene_IncestLover>() != null)
				{
					__result = false;
					return false;
				}
				return true;
			}

		}

		[HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor")]
		public static class Patch_Pawn_RelationsTracker_SecondaryLovinChanceFactor
		{

			[HarmonyPostfix]
			public static void Postfix(ref float __result, Pawn ___pawn, ref Pawn otherPawn, Pawn_RelationsTracker __instance)
			{
				if ( __instance.FamilyByBlood.Contains(otherPawn) && ___pawn?.genes?.GetFirstGeneOfType<Gene_IncestLover>() != null)
				{
					__result *= 100f;
				}
			}

		}

	}

}
