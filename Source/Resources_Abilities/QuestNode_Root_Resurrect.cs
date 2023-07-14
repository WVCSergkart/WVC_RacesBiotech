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

	public class QuestNode_Root_Resurrect : QuestNode
	{
		public int delayTicks = 150;

		protected override void RunInt()
		{
			Slate slate = QuestGen.slate;
			Quest quest = QuestGen.quest;
			Pawn pawn = slate.Get<Pawn>("asker");
			slate.Set("mechanitor", pawn);
			Gene_Undead gene_Undead = pawn.genes?.GetFirstGeneOfType<Gene_Undead>();
			quest.Delay(delayTicks, delegate
			{
				if (gene_Undead.EnoughCellsForResurrection)
				{
					gene_Undead.Gene_ResurgentCells.Value -= gene_Undead.def.resourceLossPerDay;
					UndeadUtility.Resurrect(pawn);
				}
				else if (gene_Undead.EnoughAgeForResurrection)
				{
					UndeadUtility.ResurrectWithPenalties(pawn, gene_Undead.Limit, gene_Undead.Penalty, gene_Undead.ChildBackstoryDef, gene_Undead.AdultBackstoryDef, gene_Undead.penaltyYears);
				}
				// quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, Gen.YieldSingle(pawn), filterDeadPawnsFromLookTargets: false, "[arrivalLetterText]", null, "[arrivalLetterLabel]");
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
