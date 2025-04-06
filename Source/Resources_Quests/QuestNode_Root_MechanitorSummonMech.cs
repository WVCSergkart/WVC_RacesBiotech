// RimWorld.QuestGen.QuestNode_Root_MechanitorStartingMech
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class QuestNode_Root_MechanitorSummonMech : QuestNode
	{

		public int delayTicks = 150;

		public List<PawnKindDef> mechTypes;

		public List<HediffDef> hediffDefs;

		public List<MechWeightClass> allowedMechWeightClasses;

		public bool summonGolems = false;
		//public bool summonLightMechs = false;
		//public bool summonMediumMechs = false;
		//public bool summonHeavyMechs = false;
		//public bool summonUltraHeavyMechs = false;

		protected override void RunInt()
		{
			Slate slate = QuestGen.slate;
			Quest quest = QuestGen.quest;
			Pawn pawn = slate.Get<Pawn>("asker");
			PawnKindDef pawnKindDef = MechanoidKind();
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
				quest.DropPods(map.Parent, Gen.YieldSingle(mech), null, null, null, null, false, useTradeDropSpot: true, joinPlayer: false, makePrisoners: false, null, null, QuestPart.SignalListenMode.OngoingOnly, null);
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

		private PawnKindDef MechanoidKind()
		{
			if (summonGolems)
			{
				return ListsUtility.GetAllSummonableGolems().RandomElement();
			}
			List<PawnKindDef> pawnKindDefs;
			if (!mechTypes.NullOrEmpty())
			{
				pawnKindDefs = mechTypes;
			}
			else
			{
				pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef randomXenotypeDef) => MechanoidsUtility.MechanoidIsPlayerMechanoid(randomXenotypeDef, allowedMechWeightClasses)).ToList();
			}
			return pawnKindDefs.RandomElement();
		}
	}

}
