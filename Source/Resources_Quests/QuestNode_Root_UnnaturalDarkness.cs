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

		private static readonly FloatRange InitialPhaseDurationDaysRange = new(0.5f, 0.75f);

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
			float points = slate.Get("points", 0f);
			List<Thing> list = new();
			//string questTagToAdd = QuestGenUtility.HardcodedTargetQuestTagWithQuestID("noctolith");
			string mainPhaseBeganSignal = QuestGen.GenerateNewSignal("MainPhaseBegan");
			string mainPhaseEndedSignal = QuestGen.GenerateNewSignal("MainPhaseEnded");
			//string text = QuestGen.GenerateNewSignal("NoctolRaid");
			//string inSignalNoctolithDamaged = QuestGenUtility.HardcodedSignalWithQuestID("noctolith.TookDamage");
			//string inSignalNoctolithKilled = QuestGenUtility.HardcodedSignalWithQuestID("noctolith.Destroyed");
			int num = Mathf.RoundToInt(InitialPhaseDurationDaysRange.RandomInRange * 60000f);
			int delayTicks = Mathf.RoundToInt(MainPhaseDurationDaysRange.RandomInRange * 60000f);
			quest.Letter(LetterDefOf.NegativeEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, list, filterDeadPawnsFromLookTargets: false, "[initialPhaseLetterText]", null, "[initialPhaseLetterLabel]");
			quest.GameCondition(GameConditionDefOf.UnnaturalDarkness, -1, map.Parent, permanent: true, forceDisplayAsDuration: true, null, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: false);
			quest.Delay(num - 10000, delegate
			{
				quest.Letter(LetterDefOf.NeutralEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, label: "DarknessWarningLetterLabel".Translate(), text: "DarknessWarningLetterText".Translate());
			}, null, mainPhaseBeganSignal).debugLabel = "Warning letter delay";
			quest.Delay(num, delegate
			{
				quest.SignalPass(null, null, mainPhaseBeganSignal);
			}).debugLabel = "Main phase delay";
			quest.AddPart(new QuestPart_DarkenMap(mainPhaseBeganSignal, map.Parent));
			quest.Letter(LetterDefOf.ThreatBig, mainPhaseBeganSignal, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[mainPhaseLetterText]", null, "[mainPhaseLetterLabel]");
			//List<CellRect> list2 = new();
			//LargeBuildingSpawnParms parms = NoctolithSpawnParms.ForThing(ThingDefOf.Noctolith);
			//for (int i = 0; i < 3; i++)
			//{
			//	if (!LargeBuildingCellFinder.TryFindCell(out var cell, map, parms, list2))
			//	{
			//		break;
			//	}
			//	list2.Add(CellRect.CenteredOn(cell, ThingDefOf.Noctolith.Size));
			//	Thing thing = ThingMaker.MakeThing(ThingDefOf.Noctolith);
			//	thing.SetFaction(Faction.OfEntities);
			//	QuestUtility.AddQuestTag(ref thing.questTags, questTagToAdd);
			//	list.Add(thing);
			//	quest.SpawnSkyfaller(map, ThingDefOf.NoctolithIncoming, Gen.YieldSingle(thing), null, cell, mainPhaseBeganSignal);
			//}
			//quest.AddPart(new QuestPart_Noctoliths(map.Parent, list, points, inSignalNoctolithDamaged, inSignalNoctolithKilled));
			//quest.AddPart(new QuestPart_RandomWaves(mainPhaseBeganSignal, text, 36f, 36f));
			//quest.SignalPass(delegate
			//{
			//	quest.Raid(map, points * 0.5f, Faction.OfEntities, null, raidArrivalMode: PawnsArrivalModeDefOf.EdgeWalkInDarkness, raidStrategy: RaidStrategyDefOf.ImmediateAttack, pawnGroupKind: PawnGroupKindDefOf.Noctols, customLetterLabel: "NoctolAttackLetterLabel".Translate(), customLetterText: "NoctolAttackLetterText".Translate(), customLetterLabelRules: null, customLetterTextRules: null, walkInSpot: null, tag: null, inSignal: null, rootSymbol: "root", silent: false, canTimeoutOrFlee: false, canSteal: false, canKidnap: false);
			//}, text);
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
