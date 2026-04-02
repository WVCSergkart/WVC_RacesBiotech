// RimWorld.QuestGen.QuestNode_Root_MechanitorStartingMech
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
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
			Pawn asker = slate.Get<Pawn>("asker");
			List<Map> maps = Find.Maps;
			if (!maps.Where((Map homeMape) => homeMape.IsPlayerHome).TryRandomElement(out Map map) || map == null)
			{
				QuestGen_End.End(quest, QuestEndOutcome.Success);
				return;
			}
			PawnGenerationRequest request = DuplicateUtility.RequestCopy(asker);
			Pawn reincarnated = quest.GeneratePawn(request);
			slate.Set("mechanitor", asker);
			slate.Set("reincarnated", reincarnated);
			DuplicateUtility.DuplicatePawn(asker, reincarnated);
			AgelessUtility.ChronoCorrection(reincarnated, asker);
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
