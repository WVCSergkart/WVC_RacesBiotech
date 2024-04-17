// RimWorld.QuestGen.QuestNode_Root_MechanitorStartingMech
using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

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
			List<Map> maps = Find.Maps;
			Map map = maps.Where((Map homeMape) => homeMape.IsPlayerHome).RandomElement();
			PawnGenerationRequest request = new(pawn.kindDef, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult);
			Pawn reincarnated = quest.GeneratePawn(request);
			slate.Set("mechanitor", pawn);
			slate.Set("reincarnated", reincarnated);
			GestationUtility.GeneTransfer(reincarnated, pawn, true, true);
			reincarnated.playerSettings.AreaRestriction = pawn.playerSettings.AreaRestriction;
			reincarnated.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawn);
			AgelessUtility.ChronoCorrection(reincarnated, pawn);
			AgelessUtility.Rejuvenation(reincarnated);
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
			quest.Delay(delayTicks, delegate
			{
				quest.DropPods(map.Parent, Gen.YieldSingle(reincarnated), null, null, null, null, false, useTradeDropSpot: true, joinPlayer: false, makePrisoners: false, null, null, QuestPart.SignalListenMode.OngoingOnly, null);
				quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, Gen.YieldSingle(reincarnated), filterDeadPawnsFromLookTargets: false, "[arrivalLetterText]", null, "[arrivalLetterLabel]");
				QuestGen_End.End(quest, QuestEndOutcome.Success);
			});
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
