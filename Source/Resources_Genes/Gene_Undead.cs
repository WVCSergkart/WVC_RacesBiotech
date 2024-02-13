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

		public GeneExtension_Spawner Spawner => def.GetModExtension<GeneExtension_Spawner>();

		// public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.childBackstoryDef;
		// public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.adultBackstoryDef;

		private List<HediffDef> cachedPreventiveHediffs;
		// private Gene_Dust cachedDustogenicGene = null;
		// private bool recacheDustogenicGene = true;
			// Gene_Dust gene_Dust = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();

		// public QuestScriptDef SummonQuest => def.GetModExtension<GeneExtension_Spawner>().summonQuest;
		// public int MinChronoAge => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public bool UndeadCanResurrect => PawnCanResurrect();
		public bool UndeadCanReincarnate => DustogenicCanReincarnate();

		// Getter
		public List<HediffDef> PreventResurrectionHediffs
		{
			get
			{
				if (cachedPreventiveHediffs == null)
				{
					cachedPreventiveHediffs = XenotypeFilterUtility.HediffsThatPreventUndeadResurrection();
				}
				return cachedPreventiveHediffs;
			}
		}

		public Gene_Dust Gene_Dust => pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
		// {
			// get
			// {
				// if (cachedDustogenicGene == null)
				// {
					// cachedDustogenicGene = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
				// }
				// return cachedDustogenicGene;
			// }
		// }

		// Misc
		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			if (DustogenicCanReincarnate())
			{
				Gene_DustReincarnation.Reincarnate(pawn, Spawner.summonQuest);
			}
		}

		private bool GeneIsActive()
		{
			if (!Active || Overridden || (!pawn.IsColonist && !WVC_Biotech.settings.canNonPlayerPawnResurrect))
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

		private bool DustogenicCanReincarnate()
		{
			if (!PawnCanResurrect() && Gene_Dust != null)
			{
				return GeneIsActive() && Gene_DustReincarnation.CanReincarnate(pawn, this, Spawner.stackCount);
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
			+ "WVC_XaG_Gene_DisplayStats_Undead_CanResurrectHediffs_Desc".Translate() + ":"
			+ "\n"
			+ PreventResurrectionHediffs.Select((HediffDef x) => x.label).ToLineList("  - ", capitalizeItems: true),
			1100);
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), DustogenicCanReincarnate().ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(), 1090);
			// yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanShapeshift".Translate().CapitalizeFirst(), SubXenotypeUtility.TestXenotype(pawn).ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanShapeshift_Desc".Translate(), 150);
		}

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// dustogenicGene = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
		// }

	}

	public class Gene_DustReincarnation : Gene
	{
		// public QuestScriptDef SummonQuest => def.GetModExtension<GeneExtension_Spawner>().summonQuest;
		// public int MinChronoAge => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public GeneExtension_Spawner Spawner => def.GetModExtension<GeneExtension_Spawner>();

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			// Gene_Dust gene_Dust = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			if (CanReincarnate(pawn, this, Spawner.stackCount))
			{
				return;
			}
			Reincarnate(pawn, Spawner.summonQuest);
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
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), CanReincarnate(pawn, this, Spawner.stackCount).ToString(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(), 1090);
		}

	}

}
