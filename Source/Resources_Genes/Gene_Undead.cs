using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
// using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Undead : Gene, IGeneInspectInfo
	{

		public GeneExtension_Undead Giver => def.GetModExtension<GeneExtension_Undead>();

		[Unsaved(false)]
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
			+  PreventResurrectionHediffs.Select((HediffDef x) => x.label).ToLineList("	 - ", capitalizeItems: true)) : "WVC_XaG_Gene_DisplayStats_Undead_AlwaysResurrect_Desc".Translate()),
			1100);
		}

		public string GetInspectInfo
		{
			get
			{
				if (UndeadCanResurrect)
				{
					return "WVC_XaG_Gene_Undead_On_Info".Translate().Resolve();
				}
				return null;
			}
		}

	}

	public class Gene_Reincarnation : Gene
	{

		public GeneExtension_Undead Spawner => def.GetModExtension<GeneExtension_Undead>();

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			base.Notify_PawnDied(dinfo, culprit);
			if (!ReincarnationActive())
			{
				return;
			}
			TryReincarnate(pawn, Spawner.summonQuest, WVC_Biotech.settings.reincarnation_Chance);
		}

		public bool ReincarnationActive()
		{
			return CanReincarnate(pawn, this, WVC_Biotech.settings.reincarnation_MinChronoAge);
		}

		public static bool CanReincarnate(Pawn pawn, Gene gene, float minChronoAge)
		{
			if (!WVC_Biotech.settings.reincarnation_EnableMechanic)
			{
				return false;
			}
			if (gene.Active && pawn.Faction != null && pawn.Faction == Faction.OfPlayer && pawn.ageTracker.AgeChronologicalYears > minChronoAge)
			{
				return true;
			}
			return false;
		}

		public static bool TryReincarnate(Pawn pawn, QuestScriptDef summonQuest, float reincarnationChance = 0.01f)
		{
			if (!Rand.Chance(reincarnationChance))
			{
				return false;
			}
			int litterSize = ((pawn.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.litterSizeCurve)));
			if (litterSize < 1)
			{
				litterSize = 1;
			}
			for (int i = 0; i < litterSize; i++)
			{
				ReincarnationQuest(pawn, summonQuest);
			}
			return true;
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

	public class Gene_Cellular : Gene_AddOrRemoveHediff
	{

		public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

		[Unsaved(false)]
		private float cachedRegeneration = -1;

		public float Regeneration
		{
			get
			{
				if (cachedRegeneration == -1)
				{
					cachedRegeneration = Undead.regeneration;
				}
				return cachedRegeneration;
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(617))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, Regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 617);
		}

	}

}
