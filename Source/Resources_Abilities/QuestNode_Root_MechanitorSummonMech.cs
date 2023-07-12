// RimWorld.QuestGen.QuestNode_Root_MechanitorStartingMech
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class QuestNode_Root_MechanitorSummonMech : QuestNode
	{
		private readonly int delayTicks = 150;

		public List<PawnKindDef> mechTypes;

		public List<HediffDef> hediffDefs;

		protected override void RunInt()
		{
			Slate slate = QuestGen.slate;
			Quest quest = QuestGen.quest;
			Pawn pawn = slate.Get<Pawn>("asker");
			PawnKindDef pawnKindDef = mechTypes.RandomElement();
			Map map = pawn.MapHeld;
			PawnGenerationRequest request = new(pawnKindDef, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
			Pawn mech = quest.GeneratePawn(request);
			slate.Set("mechanitor", pawn);
			slate.Set("mech", pawnKindDef);
			if (hediffDefs != null)
			{
				foreach (HediffDef hediff in hediffDefs)
				{
					if (!mech.health.hediffSet.HasHediff(hediff))
					{
						mech.health.AddHediff(hediff);
					}
				}
			}
			quest.Delay(delayTicks, delegate
			{
				quest.AssignMechToMechanitor(pawn, mech);
				quest.DropPods(map.Parent, Gen.YieldSingle(mech), null, null, null, null, false, useTradeDropSpot: false, joinPlayer: false, makePrisoners: false, null, null, QuestPart.SignalListenMode.OngoingOnly, pawn.PositionHeld);
				quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, Gen.YieldSingle(mech), filterDeadPawnsFromLookTargets: false, "[arrivalLetterText]", null, "[arrivalLetterLabel]");
				QuestGen_End.End(quest, QuestEndOutcome.Success);
			});
			quest.End(QuestEndOutcome.Fail, 0, null, QuestGenUtility.HardcodedSignalWithQuestID("asker.Killed"));
		}

		protected override bool TestRunInt(Slate slate)
		{
			Pawn pawn = slate.Get<Pawn>("asker");
			if (pawn != null && pawn.SpawnedOrAnyParentSpawned)
			{
				return MechanitorUtility.IsMechanitor(pawn);
			}
			return false;
		}

		// private PawnKindDef MechanoidKind()
		// {
			// PawnKindDef pawnKindDef;
			// pawnKindDef = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef randomXenotypeDef) => randomXenotypeDef.race.race.IsMechanoid 
			// && randomXenotypeDef.defName.Contains("Mech_") 
			// && !randomXenotypeDef.defName.Contains("TEST") 
			// && !randomXenotypeDef.defName.Contains("NonPlayer") 
			// && !randomXenotypeDef.defName.Contains("Random") 
			// && randomXenotypeDef.race.race.thinkTreeMain.defName.Contains("Mechanoid") 
			// && randomXenotypeDef.race.race.maxMechEnergy == 100
			// && randomXenotypeDef.race.race.lifeStageAges.Count > 1 
			// ).RandomElement();
			// if (mechTypes != null)
			// {
				// pawnKindDef = mechTypes.RandomElement();
			// }
			// return pawnKindDef;
		// }
	}

}
