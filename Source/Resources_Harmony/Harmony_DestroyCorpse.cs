using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(ThingDefGenerator_Corpses), "ImpliedCorpseDefs")]
		public static class Patch_ThingDefGenerator_Corpses_ImpliedCorpseDefs
		{

			public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> __result)
			{
				foreach (ThingDef thingDef in __result)
				{
					if (!SkipDef(thingDef))
					{
						yield return thingDef;
					}
				}
			}

			public static bool SkipDef(ThingDef thingDef)
			{
				GeneExtension_General modExtension = thingDef?.ingestible?.sourceDef?.GetModExtension<GeneExtension_General>();
				if (modExtension != null && modExtension.removeButcherRecipes)
				{
					return true;
				}
				return false;
			}
		}

		// [HarmonyPatch(typeof(ThingDefGenerator_Corpses), "GenerateCorpseDef")]
		// public static class Patch_ThingDefGenerator_Corpses_GenerateCorpseDef
		// {

			// [HarmonyPrefix]
			// public static bool Prefix(ref ThingDef __result, ref ThingDef pawnDef)
			// {
				// if (SkipDef(pawnDef))
				// {
					// __result = null;
					// return false;
				// }
				// return true;
			// }

			// public static bool SkipDef(ThingDef thingDef)
			// {
				// GeneExtension_General modExtension = thingDef?.GetModExtension<GeneExtension_General>();
				// if (modExtension != null && modExtension.removeButcherRecipes)
				// {
					// return true;
				// }
				// return false;
			// }
		// }

	}

}
