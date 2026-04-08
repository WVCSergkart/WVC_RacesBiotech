using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using WVC_XenotypesAndGenes.HarmonyPatches;

namespace WVC_XenotypesAndGenes
{

	//public class Gene_BodySize : XaG_Gene, IGeneOverriddenBy
	//{

	//	private static HashSet<Pawn> cachedPawns;
	//	public static HashSet<Pawn> ResizedPawns
	//	{
	//		get
	//		{
	//			if (cachedPawns == null)
	//			{
	//				List<Pawn> list = new();
	//				foreach (Pawn pawn in PawnsFinder.All_AliveOrDead)
	//				{
	//					if (pawn?.genes?.GetFirstGeneOfType<Gene_BodySize>()?.Props != null)
	//					{
	//						list.Add(pawn);
	//					}
	//				}
	//				cachedPawns = [.. list];
	//			}
	//			return cachedPawns;
	//		}
	//	}

	//	private GeneExtension_Giver cachedGeneExtension;
	//	public GeneExtension_Giver Props
	//	{
	//		get
	//		{
	//			if (cachedGeneExtension == null)
	//			{
	//				cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
	//			}
	//			return cachedGeneExtension;
	//		}
	//	}

	//	public override void PostAdd()
	//	{
	//		base.PostAdd();
	//		HarmonyPatch();
	//	}

	//	public void Notify_OverriddenBy(Gene overriddenBy)
	//	{

	//	}

	//	public void Notify_Override()
	//	{
	//		HarmonyPatch();
	//	}


	//	private static bool gamePatched = false;
	//	public static void HarmonyPatch()
	//	{
	//		if (gamePatched)
	//		{
	//			return;
	//		}
	//		try
	//		{
	//			HarmonyUtility.Harmony.Patch(AccessTools.DeclaredPropertyGetter(typeof(Pawn), "BodySize"), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.BodySizePatch))));
	//		}
	//		catch (Exception arg)
	//		{
	//			Log.Error("Failed apply predator repellent patch. Reason: " + arg.Message);
	//		}
	//		gamePatched = true;
	//	}

	//}

	public class Gene_Traitless : XaG_Gene, IGeneNotifyLifeStageStarted, IGeneOverriddenBy
	{

		public override void PostAdd()
		{
			base.PostAdd();
			UpdTraits();
		}

		private void UpdTraits()
		{
			try
			{
				TraitsUtility.RemoveAllTraits(pawn);
			}
			catch (Exception arg)
			{
				Log.Error("Failed remove traits. GeneDef: " + def.defName + ". Reason: " + arg.Message);
			}
		}

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(55555, delta))
			{
				UpdTraits();
			}
		}

		public void Notify_LifeStageStarted()
		{
			UpdTraits();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{

		}

		public void Notify_Override()
		{
			UpdTraits();
		}

	}

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


		private static bool isAcceptablePreyForPatched = false;
		public static void HarmonyPatch()
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


		private static bool incestCheckPatched = false;
		private static void HarmonyPatch()
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

		//public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		//{
		//	foreach (GeneDef geneDef in pregnancy.geneSet.GenesListForReading.ToList())
		//	{
		//		if (GeneDefOf.Inbred == geneDef)
		//		{
		//			pregnancy.geneSet.Debug_RemoveGene(geneDef);
		//		}
		//	}
		//}

		//public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		//{
		//	return false;
		//}

		//public void Hook_TicksToNextLovin(Pawn caller, Pawn partner)
		//{
		//	if (caller.gender == Gender.Female && partner.gender == Gender.Female)
		//	{
		//		if (MiscUtility.CanStartPregnancy(partner, false))
		//		{
		//			GestationUtility.Impregnate(partner);
		//		}
		//	}
		//}

	}

}
