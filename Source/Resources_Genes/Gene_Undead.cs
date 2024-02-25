using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
// using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Undead : Gene
	{

		public GeneExtension_Giver Giver => def.GetModExtension<GeneExtension_Giver>();

		private List<HediffDef> cachedPreventiveHediffs;

		public bool UndeadCanResurrect => PawnCanResurrect();

		// Getter
		public List<HediffDef> PreventResurrectionHediffs
		{
			get
			{
				if (cachedPreventiveHediffs == null)
				{
					cachedPreventiveHediffs = Giver.hediffDefs;
				}
				return cachedPreventiveHediffs;
			}
		}

		private bool GeneIsActive()
		{
			if (!Active || Overridden)
			{
				return false;
			}
			return true;
		}

		private bool PawnCanResurrect()
		{
			if (GeneIsActive())
			{
				if (!HediffUtility.HasAnyHediff(PreventResurrectionHediffs, pawn))
				{
					return true;
				}
			}
			return false;
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, 
			"WVC_XaG_Gene_DisplayStats_Undead_CanResurrect".Translate().CapitalizeFirst(),
			PawnCanResurrect().ToString(), 
			"WVC_XaG_Gene_DisplayStats_Undead_CanResurrect_Desc".Translate()
			+ "\n\n"
			+ (PreventResurrectionHediffs != null ? ("WVC_XaG_Gene_DisplayStats_Undead_CanResurrectHediffs_Desc".Translate() + ":"
			+ "\n"
			+  PreventResurrectionHediffs.Select((HediffDef x) => x.label).ToLineList("  - ", capitalizeItems: true)) : "WVC_XaG_Gene_DisplayStats_Undead_AlwaysResurrect_Desc".Translate()),
			1100);
		}

	}

	public class Gene_DustReincarnation : Gene
	{

		public GeneExtension_Spawner Spawner => def.GetModExtension<GeneExtension_Spawner>();

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			if (ReincarnationActive())
			{
				return;
			}
			Reincarnate(pawn, Spawner.summonQuest);
		}

		public bool ReincarnationActive()
		{
			return CanReincarnate(pawn, this, Spawner.stackCount);
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
			slate.Set("asker", pawn);
			_ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), CanReincarnate(pawn, this, Spawner.stackCount).ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(), 1090);
		}

	}

}
