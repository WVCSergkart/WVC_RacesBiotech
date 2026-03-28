using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{
	public class Gene_IncestLover : XaG_Gene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			HarmonyPatch();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				HarmonyPatch();
			}
		}

		//=================


		private static bool gamePatched = false;
		private static void HarmonyPatch()
		{
			if (gamePatched)
			{
				return;
			}
			try
			{
				if (WVC_Biotech.settings.enableIncestLoverGene)
				{
					HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(RelationsUtility), "Incestuous"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Incestuous_Relations))));
					HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Incestuous_LovinChanceFactor))));
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply incest lover patch. Reason: " + arg.Message);
			}
			gamePatched = true;
		}


	}

}
