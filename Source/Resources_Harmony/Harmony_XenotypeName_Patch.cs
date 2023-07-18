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

	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(Pawn_GeneTracker), "XenotypeLabel", MethodType.Getter)]
		public static class Pawn_GeneTracker_XenotypeLabel_Patch
		{
			public static void Postfix(ref string __result, Pawn_GeneTracker __instance)
			{
				if (SubXenotypeUtility.XenotypeIsSubXenotype(__instance))
				{
					XenotypeExtension_SubXenotype modExtension = __instance.Xenotype?.GetModExtension<XenotypeExtension_SubXenotype>();
					string xenotypeName = SubXenotypeUtility.GetFirstSubXenotypeName(__instance.iconDef, modExtension);
					if (xenotypeName != null)
					{
						// __result = __instance.Xenotype.label + " (" + xenotypeName.CapitalizeFirst() + ")";
						__result = xenotypeName;
					}
				}
			}
		}

		[HarmonyPatch(typeof(Pawn_GeneTracker), "XenotypeDescShort", MethodType.Getter)]
		public static class Pawn_GeneTracker_XenotypeDescShort_Patch
		{
			public static void Postfix(ref string __result, Pawn_GeneTracker __instance)
			{
				if (SubXenotypeUtility.XenotypeIsSubXenotype(__instance))
				{
					XenotypeExtension_SubXenotype modExtension = __instance.Xenotype?.GetModExtension<XenotypeExtension_SubXenotype>();
					string description = SubXenotypeUtility.GetFirstSubXenotypeDesc(__instance.iconDef, modExtension);
					if (description != null)
					{
						__result = description;
					}
				}
			}
		}

	}

}
