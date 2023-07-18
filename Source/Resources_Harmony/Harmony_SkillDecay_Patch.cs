using HarmonyLib;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(SkillRecord), "Interval")]
		public class WVC_SkillRecord_Interval_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(Pawn ___pawn)
			{
				if (MechanoidizationUtility.PawnSkillsNotDecay(___pawn))
				{
					return false;
				}
				return true;
			}
		}

			// public static bool Prefix(ref float xp, bool direct, SkillRecord __instance)
			// {
				// WVC_BiotechSettings settings = WVC_Biotech.settings;
				// if (settings.enableStatSkillFactor)
				// {
					// if (xp < 0f)
					// {
						// if (__instance.Pawn.GetStatValue(WVC_GenesDefOf.WVC_MinSkillLevel) + 1f > (float)__instance.levelInt)
						// {
							// xp *= __instance.Pawn.GetStatValue(WVC_GenesDefOf.WVC_SkillsDecayFactor);
						// }
					// }
				// }
				// if (xp < 0f)
				// {
					// if (MechanoidizationUtility.PawnSkillsNotDecay(__instance.Pawn))
					// {
						// xp *= 0;
					// }
				// }
				// return true;
			// }

		// [HarmonyPostfix]
		// public static void Postfix(ref float xp, bool direct, SkillRecord __instance)
		// {
			// xp *= __instance.pawn.GetStatValue(WVC_GenesDefOf.WVC_SkillsDecayFactor);
		// }

		// [HarmonyPrefix]
		// public static bool Prefix(ref float xp, bool direct, SkillRecord __instance)
		// {
			// TweaksGaloreSettings settings = TweaksGaloreMod.settings;
			// if (settings.tweak_skillRates)
			// {
				// if (xp < 0f)
				// {
					// if (settings.tweak_skillRateLossThreshold - 1f > (float)__instance.levelInt)
					// {
						// return false;
					// }
					// xp *= settings.tweak_skillRateLoss;
				// }
				// else
				// {
					// xp *= settings.tweak_skillRateGain;
				// }
			// }
			// return true;
		// }

	}
}
