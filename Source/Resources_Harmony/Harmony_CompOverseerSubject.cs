using HarmonyLib;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(CompOverseerSubject), "State", MethodType.Getter)]
		public static class Patch_CompOverseerSubject_State
		{

			public static bool Prefix(ref OverseerSubjectState __result, CompOverseerSubject __instance)
			{
				if (__instance.Parent.GetStatValue(StatDefOf.BandwidthCost) <= 0f)
				{
					if (__instance.Parent.GetOverseer() == null)
					{
						__result = OverseerSubjectState.RequiresOverseer;
						return false;
					}
					__result = OverseerSubjectState.Overseen;
					return false;
				}
				return true;
			}

		}

		[HarmonyPatch(typeof(CompOverseerSubject), "TryMakeFeral")]
		public static class Patch_CompOverseerSubject_TryMakeFeral
		{

			public static bool Prefix(ref bool __result, CompOverseerSubject __instance)
			{
				if (GolemsUtility.MechanoidIsGolemlike(__instance.Parent))
				{
					__result = false;
					return false;
				}
				return true;
			}

		}

		// [HarmonyPatch(typeof(MechanitorUtility), "IsPlayerOverseerSubject")]
		// public static class Patch_MechanitorUtility_IsPlayerOverseerSubject
		// {

			// public static bool Prefix(ref bool __result, ref Pawn pawn)
			// {
				// if (GolemsUtility.MechanoidIsGolemlike(pawn))
				// {
					// __result = true;
					// return false;
				// }
				// return true;
			// }

		// }

	}

}
