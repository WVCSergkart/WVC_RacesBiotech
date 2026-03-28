using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{

	public class Gene_PredatorRepellent : XaG_Gene, IGeneOverriddenBy, IGeneNotifyGenesChanged
	{

		private static HashSet<Pawn> cachedNonPreyPawns;
		public static HashSet<Pawn> NonPreyPawns
		{
			get
			{
				if (cachedNonPreyPawns == null)
				{
					HashSet<Pawn> list = new();
					foreach (Pawn pawn in PawnsFinder.All_AliveOrDead)
					{
						if (pawn?.genes?.GetFirstGeneOfType<Gene_PredatorRepellent>() != null)
						{
							list.Add(pawn);
						}
					}
					cachedNonPreyPawns = list;
				}
				return cachedNonPreyPawns;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			HarmonyPatch();
			ResetCollection();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			ResetCollection();
		}

		public void Notify_Override()
		{
			ResetCollection();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			ResetCollection();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				HarmonyPatch();
			}
		}

		public void Notify_GenesChanged(Gene changedGene)
		{
			ResetCollection();
		}

		public static void ResetCollection()
		{
			cachedNonPreyPawns = null;
		}

		//=================


		private static bool gamePatched = false;
		public static void HarmonyPatch()
		{
			if (gamePatched)
			{
				return;
			}
			try
			{
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(FoodUtility), "IsAcceptablePreyFor"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.IsNotAcceptablePrey))));
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply predator repellent patch. Reason: " + arg.Message);
			}
			gamePatched = true;
		}
	}

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
