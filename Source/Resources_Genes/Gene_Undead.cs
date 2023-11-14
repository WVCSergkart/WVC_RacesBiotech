using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
// using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Undead : Gene
	{
		// public readonly int penaltyYears = 5;

		// public readonly float oneYear = 3600000f;

		// public int Penalty => (int)(oneYear * penaltyYears);
		// public long Limit => (long)(oneYear * MinAge);
		// public float CurrentAge => pawn.ageTracker.AgeBiologicalTicks;
		// public float MinAge => pawn.ageTracker.AdultMinAge;

		// public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.childBackstoryDef;
		public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.adultBackstoryDef;

		// public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
		// public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		public Gene_ResurgentCells Gene_ResurgentCells => pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
		public Gene_Dust Gene_Dust => pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
		public Gene_Scarifier Gene_Scarifier => pawn.genes?.GetFirstGeneOfType<Gene_Scarifier>();

		// public Gene_DustReincarnation gene_DustReincarnation;
		public QuestScriptDef SummonQuest => def.GetModExtension<GeneExtension_Spawner>().summonQuest;
		// public QuestScriptDef ResurrectionQuest => def.GetModExtension<GeneExtension_Spawner>().resurrectionQuest;
		public int MinChronoAge => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public bool UndeadCanResurrect => PawnCanResurrect();
		public bool UndeadCanReincarnate => DustogenicCanReincarnate();
		public bool UndeadResourceIsActive => AnyResourceIsActive();

		public List<HediffDef> PreventResurrectionHediffs => XenotypeFilterUtility.HediffsThatPreventUndeadResurrection();

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			// pawn.health.AddHediff(HediffDefName);
			if (DustogenicCanReincarnate())
			{
				Gene_DustReincarnation.Reincarnate(pawn, SummonQuest);
			}
			if (PawnCanResurrect())
			{
				UndeadUtility.NewUndeadResurrect(pawn, ChildBackstoryDef, AdultBackstoryDef, Gene_ResurgentCells, def.resourceLossPerDay);
				// Gene_DustReincarnation.ReincarnationQuest(pawn, ResurrectionQuest);
				// UndeadUtility.NewUndeadResurrect(pawn, ChildBackstoryDef, AdultBackstoryDef, Gene_ResurgentCells, def.resourceLossPerDay);
			}
		}

		// public static void ResurrectionQuest(Pawn pawn, QuestScriptDef quest)
		// {
			// Slate slate = new();
			// slate.Set("asker", pawn);
			// _ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
		// }

		// public bool PawnCanResurrect()
		// {
			// if ((CanResurrect() && !DustogenicCanReincarnate()) || EnoughResurgentCells())
			// {
				// return true;
			// }
			// return false;
		// }

		//For checks
		private bool GeneIsActive()
		{
			if (!Active || Overridden || (!pawn.IsColonist && !WVC_Biotech.settings.canNonPlayerPawnResurrect) || pawn.genes.HasGene(GeneDefOf.Deathless))
			{
				return false;
			}
			return true;
		}

		private bool PawnCanResurrect()
		{
			if (GeneIsActive())
			{
				if (EnoughResurgentCells())
				{
					return true;
				}
				// else if (CorrectAge())
				// {
					// return true;
				// }
				else if (!AnyResourceIsActive() && !MechanoidizationUtility.HasAnyHediff(PreventResurrectionHediffs, pawn))
				{
					return true;
				}
			}
			return false;
		}

		private bool DustogenicCanReincarnate()
		{
			if (GeneIsActive() && Gene_Dust != null)
			{
				return Gene_DustReincarnation.CanReincarnate(pawn, this, MinChronoAge);
			}
			return false;
		}

		private bool EnoughResurgentCells()
		{
			if (Gene_ResurgentCells != null)
			{
				if ((Gene_ResurgentCells.Value - def.resourceLossPerDay) >= 0f)
				{
					return true;
				}
			}
			return false;
		}

		// private bool CorrectAge()
		// {
			// if (AnyResourceIsActive())
			// {
				// return false;
			// }
			// if ((CurrentAge - Penalty) > Limit)
			// {
				// return true;
			// }
			// return false;
		// }

		private bool AnyResourceIsActive()
		{
			if (Gene_ResurgentCells != null || Gene_Dust != null || Gene_Scarifier != null)
			{
				return true;
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

	}

}
