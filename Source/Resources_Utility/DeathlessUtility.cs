using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{
	public static class DeathlessUtility
	{

		public static void ReincarnationQuest(Pawn pawn, QuestScriptDef quest)
		{
			Slate slate = new();
			slate.Set("asker", pawn);
			_ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
		}

		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========

		/// <summary>
		/// A test hook for the custom deathless mechanic.
		/// A bit more testing is needed to identify any flaws.
		/// </summary>

		//private static HashSet<Pawn> cachedDeathlessPawns;
		//public static HashSet<Pawn> DeathlessPawns
		//{
		//	get
		//	{
		//		if (cachedDeathlessPawns == null)
		//		{
		//			List<Pawn> list = new();
		//			foreach (Pawn pawn in PawnsFinder.AllMapsAndWorld_Alive)
		//			{
		//				if (pawn?.genes?.GenesListForReading?.Any(gene => gene is IGeneDeathless && gene.Active) == true)
		//				{
		//					list.Add(pawn);
		//				}
		//			}
		//			//Log.Error(list.Select((pawn) => pawn.Name.ToString()).ToLineList(" - "));
		//			cachedDeathlessPawns = [.. list];
		//		}
		//		return cachedDeathlessPawns;
		//	}
		//}

		//public static void ResetCollection()
		//{
		//	cachedDeathlessPawns = null;
		//}

		//private static bool hasActiveGeneDeathlessHookPatched = false;
		//public static void HarmonyPatch()
		//{
		//	if (hasActiveGeneDeathlessHookPatched)
		//	{
		//		return;
		//	}
		//	try
		//	{
		//		HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn_GeneTracker), "HasActiveGene"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.DeathlessHook))));
		//	}
		//	catch (Exception arg)
		//	{
		//		Log.Warning("Non-critical error. Failed apply deathless hook. Reason: " + arg.Message);
		//	}
		//	hasActiveGeneDeathlessHookPatched = true;
		//}

		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========
		// ==========COLLECTION==========COLLECTION==========COLLECTION==========

	}

}
