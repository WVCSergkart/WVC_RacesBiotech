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

		public GeneExtension_Undead Giver => def.GetModExtension<GeneExtension_Undead>();

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

		private bool PawnCanResurrect()
		{
			if (Giver.ignoreHediffs)
			{
				return base.Active;
			}
			if (!HediffUtility.HasAnyHediff(PreventResurrectionHediffs, pawn))
			{
				return base.Active;
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

	public class Gene_Reincarnation : Gene
	{

		public GeneExtension_Undead Spawner => def.GetModExtension<GeneExtension_Undead>();

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			if (!ReincarnationActive())
			{
				return;
			}
			Reincarnate(pawn, Spawner.summonQuest);
		}

		public bool ReincarnationActive()
		{
			return CanReincarnate(pawn, this, WVC_Biotech.settings.reincarnation_MinChronoAge);
		}

		public static bool CanReincarnate(Pawn pawn, Gene gene, float minChronoAge)
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
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), CanReincarnate(pawn, this, WVC_Biotech.settings.reincarnation_MinChronoAge).ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(WVC_Biotech.settings.reincarnation_MinChronoAge.ToString()), 1090);
		}

	}

}
