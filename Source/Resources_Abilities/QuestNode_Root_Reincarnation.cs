// RimWorld.QuestGen.QuestNode_Root_MechanitorStartingMech
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class QuestNode_Root_UndeadReincarnation : QuestNode
	{
		public int delayTicks = 150;

		public List<HediffDef> hediffDefs;

		protected override void RunInt()
		{
			Slate slate = QuestGen.slate;
			Quest quest = QuestGen.quest;
			Pawn pawn = slate.Get<Pawn>("asker");
			// Log.Error("1");
			// PawnKindDef pawnKindDef = mechTypes.RandomElement();
			// Map map = pawn.MapHeld;
			List <Map> maps = Find.Maps;
			Map map = maps.Where((Map homeMape) => homeMape.IsPlayerHome).RandomElement();
			// Log.Error("2");
			// PawnGenerationRequest request = new(pawnKindDef, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			PawnGenerationRequest request = new(pawn.kindDef, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult);
			Pawn reincarnated = quest.GeneratePawn(request);
			// Log.Error("3");
			slate.Set("mechanitor", pawn);
			slate.Set("reincarnated", reincarnated);
			GestationUtility.GeneTransfer(reincarnated, pawn, true, true);
			// reincarnated.skills.skills = pawn.skills.skills;
			reincarnated.playerSettings.AreaRestriction = pawn.playerSettings.AreaRestriction;
			reincarnated.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawn);
			if (reincarnated.ageTracker.AgeChronologicalTicks >= pawn.ageTracker.AgeChronologicalTicks)
			{
				if (reincarnated.ageTracker.AgeChronologicalTicks - (18L * 3600000L) < reincarnated.ageTracker.AgeBiologicalTicks)
				{
					reincarnated.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeChronologicalTicks;
				}
				else
				{
					reincarnated.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeChronologicalTicks - (18L * 3600000L);
				}
			}
			if ((3600000L * 18L) <= reincarnated.ageTracker.AgeBiologicalTicks)
			{
				reincarnated.ageTracker.AgeBiologicalTicks = (18L * 3600000L) + 100000;
			}
			// Log.Error("4");
			if (hediffDefs != null)
			{
				foreach (HediffDef hediff in hediffDefs)
				{
					if (!reincarnated.health.hediffSet.HasHediff(hediff))
					{
						reincarnated.health.AddHediff(hediff);
					}
				}
			}
			// Log.Error("5");
			quest.Delay(delayTicks, delegate
			{
				// quest.AssignMechToMechanitor(pawn, reincarnated);
				// Log.Error("6");
				quest.DropPods(map.Parent, Gen.YieldSingle(reincarnated), null, null, null, null, false, useTradeDropSpot: false, joinPlayer: false, makePrisoners: false, null, null, QuestPart.SignalListenMode.OngoingOnly, pawn.PositionHeld);
				quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, Gen.YieldSingle(reincarnated), filterDeadPawnsFromLookTargets: false, "[arrivalLetterText]", null, "[arrivalLetterLabel]");
				QuestGen_End.End(quest, QuestEndOutcome.Success);
				// Log.Error("7");
			});
			// quest.End(QuestEndOutcome.Fail, 0, null, QuestGenUtility.HardcodedSignalWithQuestID("asker.Killed"));
		}

		protected override bool TestRunInt(Slate slate)
		{
			Pawn pawn = slate.Get<Pawn>("asker");
			if (pawn != null && pawn.SpawnedOrAnyParentSpawned)
			{
				return true;
			}
			return false;
		}
	}

}
