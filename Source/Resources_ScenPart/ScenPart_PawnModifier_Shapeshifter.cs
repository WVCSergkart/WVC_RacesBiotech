// RimWorld.StatPart_Age
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using WVC_XenotypesAndGenes.HarmonyPatches;
using static System.Net.Mime.MediaTypeNames;

namespace WVC_XenotypesAndGenes
{

	//public class ScenPart_PawnModifier_Shapeshifter : ScenPart_PawnModifier
	//{

	//	public List<GeneDef> shiftGeneDefs;

	//	public HediffDef hediffDef;

	//	public XenotypeDef leperStartDef;
	//	public XenotypeDef ashenStartDef;

	//	private XenotypeDef startingXenotypeDef;

	//	private static bool? cachedScenarioCompleted;
	//	public static bool Anomaly_ScenarioCompleted
	//	{
	//		get
	//		{
	//			if (cachedScenarioCompleted == null)
	//			{
	//				if (ModsConfig.AnomalyActive)
	//				{
	//					// Permanent
	//					cachedScenarioCompleted = false;
	//				}
	//				else
	//				{
	//					cachedScenarioCompleted = MonolithLevelDefOf.Embraced == Find.Anomaly.LevelDef;
	//				}
	//			}
	//			return cachedScenarioCompleted.Value;
	//		}
	//	}

	//	private bool IsLeperStart => startingXenotypeDef == leperStartDef;
	//	private bool IsAshenStart => startingXenotypeDef == ashenStartDef;

	//	public override void PostGameStart()
	//	{
	//		base.PostGameStart();
	//		try
	//		{
	//			startingXenotypeDef = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists?.First()?.genes?.Xenotype;
	//		}
	//		catch (Exception ex)
	//		{
	//			Log.Error("Failed get starting pawn xenotype. Reason: " + ex.Message);
	//		}
	//		SetGeneral();
	//		FaultyShapeDisease();
	//	}

	//	public override void ExposeData()
	//	{
	//		base.ExposeData();
	//		Scribe_Values.Look(ref nextTick, "nextTick", 60000);
	//		Scribe_Defs.Look(ref leperStartDef, "leperStartDef");
	//		Scribe_Defs.Look(ref ashenStartDef, "ashenStartDef");
	//		Scribe_Defs.Look(ref hediffDef, "hediffDef");
	//		Scribe_Defs.Look(ref startingXenotypeDef, "startingXenotypeDef");
	//		Scribe_Collections.Look(ref shiftGeneDefs, "leperGeneDefs", LookMode.Def);
	//		if (Scribe.mode == LoadSaveMode.PostLoadInit)
	//		{
	//			SetGeneral();
	//		}
	//	}

	//	private int nextTick = 60000;
	//	public override void Tick()
	//	{
	//		if (Anomaly_ScenarioCompleted)
	//		{
	//			return;
	//		}
	//		if (GeneResourceUtility.CanTick(ref nextTick, 60000, 1))
	//		{
	//			FaultyShapeDisease();
	//			cachedScenarioCompleted = MonolithLevelDefOf.Embraced == Find.Anomaly.LevelDef;
	//			if (Anomaly_ScenarioCompleted)
	//			{
	//				Gene_Shapeshifter.xenotypesOverride = null;
	//			}
	//		}
	//	}

	//	private void FaultyShapeDisease()
	//	{
	//		try
	//		{
	//			foreach (Pawn pawn in ListsUtility.AllPlayerPawns_MapsOrCaravans_Alive)
	//			{
	//				if (!pawn.IsShapeshifter())
	//				{
	//					continue;
	//				}
	//				LeperStart(pawn);
	//			}
	//		}
	//		catch (Exception arg)
	//		{
	//			Log.Error("Failed update shapeshifter(s) disease. Reason: " + arg.Message);
	//		}

	//		void LeperStart(Pawn pawn)
	//		{
	//			if (!IsLeperStart)
	//			{
	//				return;
	//			}
	//			HediffUtility.TryAddHediff(hediffDef, pawn, null);
	//		}
	//	}

	//	public override void PostWorldGenerate()
	//	{
	//		SetupGenes();
	//	}

	//	public static List<GeneDef> scenarioGeneDefs;
	//	public static void AddedFromScenario(ref List<GeneDefWithChance> allGenes, List<GeneDef> pawnGenes, Pawn pawn)
	//	{
	//		if (ScenPart_PawnModifier_Shapeshifter.scenarioGeneDefs == null)
	//		{
	//			return;
	//		}
	//		foreach (GeneDef item in ScenPart_PawnModifier_Shapeshifter.scenarioGeneDefs)
	//		{
	//			if (item.prerequisite != null && !XaG_GeneUtility.HasActiveGene(item.prerequisite, pawn))
	//			{
	//				continue;
	//			}
	//			GeneDefWithChance geneDefWithChance = new();
	//			geneDefWithChance.geneDef = item;
	//			geneDefWithChance.disabled = pawnGenes.Contains(item);
	//			geneDefWithChance.displayCategory = GeneCategoryDefOf.Miscellaneous;
	//			if (DebugSettings.ShowDevGizmos)
	//			{
	//				geneDefWithChance.Cost = 0;
	//			}
	//			else
	//			{
	//				geneDefWithChance.Cost = 55;
	//			}
	//			allGenes.Add(geneDefWithChance);
	//		}
	//	}

	//	private void SetGeneral()
	//	{
	//		SetupGenes();
	//		if (IsLeperStart)
	//		{
	//			scenarioGeneDefs = shiftGeneDefs;
	//			//Log.Error(scenarioGeneDefs.Select(def => def.label).ToLineList(" - "));
	//		}
	//		cachedScenarioCompleted = null;
	//		if (!Anomaly_ScenarioCompleted)
	//		{
	//			Gene_Shapeshifter.xenotypesOverride = ListsUtility.GetAllXenotypesHolders().Where(xenos => !xenos.genes.Any(gene => gene.biostatArc != 0) || xenos.Baseliner).ToList();
	//		}
	//		else
	//		{
	//			Gene_Shapeshifter.xenotypesOverride = null;
	//		}
	//		HarmonyPatch_Bisexual();
	//	}

	//	private static bool genesSetted = false;
	//	private void SetupGenes()
	//	{
	//		if (genesSetted)
	//		{
	//			return;
	//		}
	//		try
	//		{
	//			foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading.Where(gene => !gene.IsAndroid()))
	//			{
	//				geneDef.biostatCpx /= 2;
	//				if (geneDef.biostatArc > 0)
	//				{
	//					int newArc = geneDef.biostatArc / 2;
	//					if (newArc < 1)
	//					{
	//						newArc = 1;
	//					}
	//					geneDef.biostatArc = newArc;
	//				}
	//			}
	//		}
	//		catch (Exception arg)
	//		{
	//			Log.Error("Failed setup genes. Reason: " + arg.Message);
	//		}
	//		genesSetted = true;
	//	}

	//	private static bool hasTraitHook_Patched = false;
	//	public static void HarmonyPatch_Bisexual()
	//	{
	//		if (hasTraitHook_Patched)
	//		{
	//			return;
	//		}
	//		try
	//		{
	//			HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(TraitSet), "HasTrait", [typeof(TraitDef)]), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.BisexualHook))));
	//			HarmonyUtility.Harmony.Patch(AccessTools.Method(typeof(TraitSet), "HasTrait", [typeof(TraitDef), typeof(int)]), postfix: new HarmonyMethod(typeof(HarmonyUtility).GetMethod(nameof(HarmonyUtility.BisexualHook))));
	//		}
	//		catch (Exception arg)
	//		{
	//			Log.Warning("Non-critical error. Failed apply bisexual hook. Reason: " + arg.Message);
	//		}
	//		hasTraitHook_Patched = true;
	//	}

	//}

}
