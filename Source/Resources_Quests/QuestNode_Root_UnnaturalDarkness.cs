using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class QuestNode_Root_UnnaturalDarkness : RimWorld.QuestGen.QuestNode_Root_UnnaturalDarkness
	{
		//public const int MaxNoctoliths = 3;

		//private static readonly FloatRange InitialPhaseDurationDaysRange = new(0.5f, 0.75f);

		private static readonly FloatRange MainPhaseDurationDaysRange = new(120f, 140f);
  //      public static readonly LargeBuildingSpawnParms NoctolithSpawnParms = new()
  //      {
		//	maxDistanceToColonyBuilding = -1f,
		//	minDistToEdge = 10,
		//	attemptSpawnLocationType = SpawnLocationType.Outdoors,
		//	attemptNotUnderBuildings = true,
		//	canSpawnOnImpassable = false
		//};

		protected override bool TestRunInt(Slate slate)
		{
			if (!ModsConfig.AnomalyActive)
			{
				return false;
			}
			Map map = QuestGen_Get.GetMap();
			if (map == null)
			{
				return false;
			}
			//LargeBuildingSpawnParms parms = NoctolithSpawnParms.ForThing(ThingDefOf.Noctolith);
   //         if (!LargeBuildingCellFinder.AnyCellFast(map, parms))
   //         {
   //             return LargeBuildingCellFinder.TryFindCell(out IntVec3 cell, map, parms);
   //         }
            return true;
		}

		protected override void RunInt()
		{
			Quest quest = QuestGen.quest;
			Slate slate = QuestGen.slate;
			Map map = QuestGen_Get.GetMap();
			slate.Set("map", map);
			//float points = slate.Get("points", 0f);
			//List<Thing> list = new();
			string mainPhaseBeganSignal = QuestGen.GenerateNewSignal("MainPhaseBegan");
			string mainPhaseEndedSignal = QuestGen.GenerateNewSignal("MainPhaseEnded");
			//int num = Mathf.RoundToInt(InitialPhaseDurationDaysRange.RandomInRange * 60000f);
			int delayTicks = Mathf.RoundToInt(MainPhaseDurationDaysRange.RandomInRange * 60000f);
			//quest.Letter(LetterDefOf.NegativeEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, list, filterDeadPawnsFromLookTargets: false, "[initialPhaseLetterText]", null, "[initialPhaseLetterLabel]");
			quest.GameCondition(GameConditionDefOf.UnnaturalDarkness, -1, map.Parent, permanent: true, forceDisplayAsDuration: true, null, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: false);
			//quest.Delay(num - 10000, delegate
			//{
			//	quest.Letter(LetterDefOf.NeutralEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, label: "DarknessWarningLetterLabel".Translate(), text: "DarknessWarningLetterText".Translate());
			//}, null, mainPhaseBeganSignal).debugLabel = "Warning letter delay";
			//quest.Delay(num, delegate
			//{
			//	quest.SignalPass(null, null, mainPhaseBeganSignal);
			//}).debugLabel = "Main phase delay";
			quest.AddPart(new QuestPart_DarkenMap(mainPhaseBeganSignal, map.Parent));
			//quest.Letter(LetterDefOf.ThreatBig, mainPhaseBeganSignal, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[mainPhaseLetterText]", null, "[mainPhaseLetterLabel]");
			quest.Delay(delayTicks, delegate
			{
				quest.SignalPass(null, null, mainPhaseEndedSignal);
			}, mainPhaseBeganSignal).debugLabel = "Main phase end delay";
			quest.Letter(LetterDefOf.PositiveEvent, mainPhaseEndedSignal, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[darknessLiftedLetterText]", null, "[darknessLiftedLetterLabel]");
			quest.AddPart(new QuestPart_DestroyAllThingsOfDef(mainPhaseEndedSignal, map.Parent, new List<ThingDef> { ThingDefOf.Noctolith }));
			quest.AddPart(new QuestPart_GiveMemoryToHumansOnMap
			{
				memory = ThoughtDefOf.DarknessLifted,
				inSignal = mainPhaseEndedSignal,
				mapParent = map.Parent
			});
			quest.SignalPass(delegate
			{
				quest.End(QuestEndOutcome.Success, 0, null, mainPhaseEndedSignal);
			}, mainPhaseEndedSignal);
			Pawn pawn = map.mapPawns.FreeColonistsSpawned.RandomElementWithFallback();
			if (pawn != null)
			{
				TaleRecorder.RecordTale(TaleDefOf.UnnaturalDarkness, pawn);
			}
		}
	}

}
