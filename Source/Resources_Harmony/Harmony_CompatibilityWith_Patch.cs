using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	[HarmonyPatch(typeof(Pawn_RelationsTracker), "CompatibilityWith")]
	public static class Pawn_RelationsTracker_CompatibilityWith_Patch
	{
		public static void Postfix(ref float __result, Pawn otherPawn, Pawn ___pawn)
		{
			if (___pawn.relations.FamilyByBlood.Contains(otherPawn) && MechanoidizationUtility.IsIncestLover(___pawn))
			{
				__result *= 10f;
			}
		}
	}

	[HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryRomanceChanceFactor")]
	public static class Pawn_RelationsTracker_SecondaryRomanceChanceFactor_Patch
	{
		public static void Postfix(ref float __result, Pawn otherPawn, Pawn ___pawn)
		{
			if (__result <= 0f && ___pawn.relations.FamilyByBlood.Contains(otherPawn) && MechanoidizationUtility.IsIncestLover(___pawn))
			{
				__result = 1.0f;
			}
		}
	}

	// TEST ONLY
	// [HarmonyPatch(typeof(RelationsUtility), "AttractedToGender")]
	// public static class Patch_RelationsUtility_AttractedToGender
	// {

		// [HarmonyPrefix]
		// public static bool Prefix(ref bool __result)
		// {
			// __result = true;
			// return false;
		// }
	// }

}
