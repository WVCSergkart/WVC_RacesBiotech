using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DustReincarnation : Gene_DustDrain
	{
		public QuestScriptDef SummonQuest => def.GetModExtension<GeneExtension_Spawner>().summonQuest;
		public int MinChronoAge => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			Gene_Dust gene_Dust = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			if (!Active || pawn.Faction != Faction.OfPlayer || gene_Dust == null || pawn.ageTracker.AgeChronologicalYears < MinChronoAge)
			{
				return;
			}
			int litterSize = ((pawn.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			for (int i = 0; i < litterSize; i++)
			{
				ReincarnationQuest(pawn, SummonQuest);
			}
		}

		public static void ReincarnationQuest(Pawn pawn, QuestScriptDef quest)
		{
			Slate slate = new();
			// slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(pawn.Map));
			slate.Set("asker", pawn);
			_ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
			// QuestUtility.SendLetterQuestAvailable(quest);
		}

	}
}
