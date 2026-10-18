using HarmonyLib;
using RimWorld;
using System;
using Verse;
using Verse.AI;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{

	public static class HotPatchesUtility
	{

		private static bool duplicatorObeliskPatched = false;
		private static bool skillRecordSkillLossPatched = false;
		private static bool isAcceptablePreyForPatched = false;
		private static bool mechanitorGetGizmosPatched = false;
		private static bool genesInOrderPatched = false;
		private static bool movementCostPatched = false;
		private static bool incestCheckPatched = false;

		public static void Patch_AcceptablePrey()
		{
			if (isAcceptablePreyForPatched)
			{
				return;
			}
			try
			{
				if (!WVC_Biotech.settings.disableNonAcceptablePreyGenes)
				{
					HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(FoodUtility), "IsAcceptablePreyFor"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.IsNotAcceptablePrey))));
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply predator repellent patch. Reason: " + arg.Message);
			}


			isAcceptablePreyForPatched = true;
		}
		public static void Patch_HideGenesInOrder()
		{
			if (genesInOrderPatched)
			{
				return;
			}
			try
			{
				if (!WVC_Biotech.settings.hideXaGGenes) //  && !WVC_Biotech.settings.EnableLegacyMode
				{
					HarmonyUtility.Harmony.Patch(AccessTools.DeclaredPropertyGetter(typeof(GeneUtility), "GenesInOrder"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Patch_HideObsoleteGenes))));
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply hideGenes patch. Reason: " + arg.Message);
			}


			genesInOrderPatched = true;
		}
		public static void Patch_IncestAndLovinChance()
		{
			if (incestCheckPatched)
			{
				return;
			}
			try
			{
				if (WVC_Biotech.settings.enableIncestLoverGene)
				{
					HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(RelationsUtility), "Incestuous"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Incestuous_Relations))));
					HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Incestuous_LovinChanceFactor))));
					//HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(JobDriver_Lovin), "GenerateRandomMinTicksToNextLovin"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.Notify_GotLovin))));
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply incest lover patch. Reason: " + arg.Message);
			}


			incestCheckPatched = true;
		}
		public static void Patch_MechanitorGizmos()
		{
			if (mechanitorGetGizmosPatched)
			{
				return;
			}
			try
			{
				//var harmony = new Harmony("wvc.sergkart.races.biotech.pergene");
				//HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn_PathFollower), "CostToMoveIntoCell", [typeof(Pawn), typeof(IntVec3)]), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.NoMovementCost))));
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn_MechanitorTracker), "GetGizmos"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.MechanitorHideWithGene))));
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply mechanitor UI patch. Reason: " + arg.Message);
			}


			mechanitorGetGizmosPatched = true;
		}
		public static void Patch_MovementCost()
		{
			if (movementCostPatched)
			{
				return;
			}
			try
			{
				//var harmony = new Harmony("wvc.sergkart.races.biotech.pergene");
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn_PathFollower), "CostToMoveIntoCell", [typeof(Pawn), typeof(IntVec3)]), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.NoMovementCost))));
				//HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(Pawn), "GetPathContext"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.PathGridPatch))));
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply wings patch. Reason: " + arg.Message);
			}


			movementCostPatched = true;
		}

		public static void Patch_ObeliskDuplicatorTweak()
		{
			if (duplicatorObeliskPatched)
			{
				return;
			}
			try
			{
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(AnomalyUtility), "TryDuplicatePawn"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.AnomalyTryDuplicate_Patch))));
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply obelisk duplicator patch. Reason: " + arg.Message);
			}


			duplicatorObeliskPatched = true;
		}
		public static void Patch_ScenarioSkillLose()
		{
			if (skillRecordSkillLossPatched)
			{
				return;
			}
			try
			{
				//var harmony = new Harmony("wvc.sergkart.races.biotech.hivemindhatred");
				HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(SkillRecord), "Interval"), prefix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.NoSkillLossPatch))));
				//harmony.Patch(AccessTools.DeclaredPropertyGetter(typeof(SkillRecord), "Aptitude"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.HivemindHatredAptitude))));
				//harmony.Patch(AccessTools.Method(typeof(SkillRecord), "DirtyAptitudes"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.HivemindHatredAptitude))));
			}
			catch (Exception arg)
			{
				Log.Error("Failed apply hatred patch. Reason: " + arg.Message);
			}


			skillRecordSkillLossPatched = true;
		}
	}
}