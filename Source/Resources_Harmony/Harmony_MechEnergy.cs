using HarmonyLib;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(Need_MechEnergy), "FallPerDay", MethodType.Getter)]
		public static class WVC_Need_MechEnergy_FallPerDay_Patch
		{

			public static bool Prefix(ref float __result, Pawn ___pawn)
			{
				if (MechanoidizationUtility.PawnIsGolem(___pawn))
				{
					__result = -1f;
					return false;
				}
				return true;
			}

		}

		[HarmonyPatch(typeof(Need_MechEnergy), "GUIChangeArrow", MethodType.Getter)]
		public static class WVC_Need_MechEnergy_GUIChangeArrow_Patch
		{

			public static bool Prefix(ref int __result, Pawn ___pawn)
			{
				if (MechanoidizationUtility.PawnIsGolem(___pawn))
				{
					__result = 1;
					return false;
				}
				return true;
			}

		}

		[HarmonyPatch(typeof(Need_MechEnergy), "GetTipString")]
		public static class WVC_Need_MechEnergy_GetTipString_Patch
		{

			public static bool Prefix(ref string __result, Pawn ___pawn)
			{
				if (MechanoidizationUtility.PawnIsGolem(___pawn))
				{
					__result = "WVC_XaG_CurrentGolemEnergy".Translate() + ": " + "+" + (0.01f).ToStringPercent();
					return false;
				}
				return true;
			}

		}

		// [HarmonyPatch(typeof(Need_MechEnergy), "LabelCap")]
		// public static class WVC_Need_MechEnergy_LabelCap_Patch
		// {

			// public static void Postfix(ref string ___LabelCap, Pawn ___pawn)
			// {
				// if (MechanoidizationUtility.PawnIsGolem(___pawn))
				// {
					// ___LabelCap = "WVC_XaG_GolemEnergyLabelCap".Translate();
				// }
			// }

		// }

	}

}
