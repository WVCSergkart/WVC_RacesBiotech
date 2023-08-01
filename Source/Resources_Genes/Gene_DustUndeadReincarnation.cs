using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_DustReincarnation : Gene
    {
        public QuestScriptDef SummonQuest => def.GetModExtension<GeneExtension_Spawner>().summonQuest;
        public int MinChronoAge => def.GetModExtension<GeneExtension_Spawner>().stackCount;

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            // Gene_Dust gene_Dust = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
            if (CanReincarnate(pawn, this, MinChronoAge))
            {
                return;
            }
            Reincarnate(pawn, SummonQuest);
        }

		public static bool CanReincarnate(Pawn pawn, Gene gene, int minChronoAge)
		{
			if (gene.Active && pawn.Faction != null && pawn.Faction == Faction.OfPlayer && pawn.ageTracker.AgeChronologicalYears > minChronoAge)
			{
				return true;
			}
			return false;
		}

        public static void Reincarnate(Pawn pawn, QuestScriptDef summonQuest)
        {
            int litterSize = ((pawn.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.litterSizeCurve)));
            if (litterSize < 1)
            {
                litterSize = 1;
            }
            for (int i = 0; i < litterSize; i++)
            {
                ReincarnationQuest(pawn, summonQuest);
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

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), CanReincarnate(pawn, this, MinChronoAge).ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(), 1090);
		}

    }
}
