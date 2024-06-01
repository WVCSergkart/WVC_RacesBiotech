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

		[HarmonyPatch(typeof(Gene), "OverrideBy")]
		public static class Patch_Gene_OverrideBy
		{
			public static void Postfix(Gene __instance, Gene overriddenBy)
			{
				if (__instance is IGeneOverridden geneOverridden)
				{
					if (overriddenBy != null)
					{
						geneOverridden.Notify_OverriddenBy(overriddenBy);
					}
					else
					{
						geneOverridden.Notify_Override();
					}
				}
				if (__instance is IGeneInspectInfo)
				{
					// Log.Error("ResetGenesInspectString");
					XaG_GeneUtility.ResetGenesInspectString(__instance.pawn);
				}
				// if (__instance is IGeneNotifyGenesChanged geneNotifyGenesChanged)
				// {
					// geneNotifyGenesChanged.Notify_GenesChanged(overriddenBy);
				// }
			}
		}

		// [HarmonyPatch(typeof(Pawn_GeneTracker), "Notify_GenesChanged")]
		// public static class Patch_Pawn_GeneTracker_Notify_GenesChanged
		// {
			// public static void Postfix(Pawn_GeneTracker __instance, GeneDef addedOrRemovedGene)
			// {
				// if (__instance is IGeneInspectInfo)
				// {
					// XaG_GeneUtility.ResetGenesInspectString(__instance.pawn);
				// }
				// if (__instance is IGeneNotifyGenesChanged geneNotifyGenesChanged)
				// {
					// geneNotifyGenesChanged.Notify_GenesChanged(addedOrRemovedGene);
				// }
			// }
		// }

	}

}
