// RimWorld.QuestGen.QuestNode_Root_MechanitorStartingMech
using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class QuestNode_Root_ChimerkinShip : QuestNode
	{

		protected string QuestTag = "ChimerkinShip";

		protected override void RunInt()
		{
			Quest quest = QuestGen.quest;
			Slate slate = QuestGen.slate;
			Map map = QuestGen_Get.GetMap();
			float x = slate.Get("points", 0f);
			//int endTicks = 5240;
			string questTagToAdd = QuestGenUtility.HardcodedTargetQuestTagWithQuestID(QuestTag);
			//string attackedSignal = QuestGenUtility.HardcodedSignalWithQuestID("shuttlePawns.TookDamageFromPlayer");
			//string defendTimeoutSignal = QuestGen.GenerateNewSignal("DefendTimeout");
			//string beginAssaultSignal = QuestGen.GenerateNewSignal("BeginAssault");
			//string assaultBeganSignal = QuestGen.GenerateNewSignal("AssaultBegan");
			slate.Set("map", map);
			List<FactionRelation> list = new();
			foreach (Faction item3 in Find.FactionManager.AllFactionsListForReading)
			{
				list.Add(new FactionRelation(item3, FactionRelationKind.Hostile));
			}
			Faction faction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Sanguophages, list, hidden: true);
			faction.temporary = true;
			Find.FactionManager.Add(faction);
			quest.ReserveFaction(faction);
			List<Pawn> shuttlePawns = new();
			Pawn baby = quest.GeneratePawn(GestationUtility.NewBornRequest(PawnKindDefOf.Sanguophage, faction));
			AgelessUtility.SetAge(baby, new IntRange(600000, 7200000).RandomInRange);
			ReimplanterUtility.SetXenotype(baby, DefDatabase<XenotypeDef>.GetNamed("WVC_Shadoweater"));
			baby.health.forceDowned = true;
			shuttlePawns.Add(baby);
			slate.Set("sanguophage", baby);
			slate.Set("shuttlePawns", shuttlePawns);
			Thing thing = ThingMaker.MakeThing(ThingDefOf.ShuttleCrashed_Exitable);
			quest.SetFaction(Gen.YieldSingle(thing), faction);
			TryFindShuttleCrashPosition(map, thing.def.size, out var shuttleCrashPosition);
			TransportShip transportShip = quest.GenerateTransportShip(TransportShipDefOf.Ship_ShuttleCrashing, shuttlePawns, thing).transportShip;
			quest.AddShipJob_WaitTime(transportShip, 60, leaveImmediatelyWhenSatisfied: false).showGizmos = false;
			quest.AddShipJob(transportShip, ShipJobDefOf.Unload);
			QuestUtility.AddQuestTag(ref transportShip.questTags, questTagToAdd);
			quest.Delay(180, delegate
			{
				quest.Letter(LetterDefOf.NeutralEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, shuttlePawns, filterDeadPawnsFromLookTargets: false, "[sanguophageShuttleCrashedLetterText]", null, "[sanguophageShuttleCrashedLetterLabel]");
				quest.AddShipJob_Arrive(transportShip, map.Parent, null, shuttleCrashPosition, ShipJobStartMode.Force_DelayCurrent, faction);
				//quest.DefendPoint(map.Parent, shuttleCrashPosition, shuttlePawns, faction, null, null, 5f);
				//quest.Delay(5000, delegate
				//{
				//	quest.SignalPass(null, null, attackedSignal);
				//}).debugLabel = "Drop delay";
				//quest.AnySignal(new string[2] { attackedSignal, defendTimeoutSignal }, null, Gen.YieldSingle(beginAssaultSignal));
				//quest.SignalPassActivable(delegate
				//{
				//	quest.AnyPawnInCombatShape(shuttlePawns, delegate
				//	{
				//		QuestPart_AssaultColony questPart_AssaultColony2 = quest.AssaultColony(faction, map.Parent, shuttlePawns);
				//		questPart_AssaultColony2.canKidnap = false;
				//		questPart_AssaultColony2.canSteal = false;
				//		questPart_AssaultColony2.canTimeoutOrFlee = false;
				//		quest.Letter(LetterDefOf.NeutralEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, shuttlePawns, filterDeadPawnsFromLookTargets: false, "[assaultBeginLetterText]", null, "[assaultBeginLetterLabel]");
				//	}, null, null, assaultBeganSignal);
				//}, null, beginAssaultSignal, null, null, assaultBeganSignal);
				//quest.Delay(endTicks, delegate
				//{
				//}).debugLabel = "End delay";
				QuestGen_End.End(quest, QuestEndOutcome.Success);
			}).debugLabel = "Arrival delay";
		}

		protected override bool TestRunInt(Slate slate)
		{
			Map map = QuestGen_Get.GetMap();
			if (map == null)
			{
				return false;
			}
			if (!TryFindShuttleCrashPosition(map, ThingDefOf.ShuttleCrashed.size, out var _))
			{
				return false;
			}
			return true;
		}

		private bool TryFindShuttleCrashPosition(Map map, IntVec2 size, out IntVec3 spot)
		{
			if (DropCellFinder.FindSafeLandingSpot(out spot, null, map, 35, 15, 25, size, ThingDefOf.ShuttleCrashed.interactionCellOffset))
			{
				return true;
			}
			return false;
		}
	}

}
