using RimWorld;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
// using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Undead : Gene, IGeneNotifyOnKilled
	{

		private Dictionary<WorkTypeDef, int> workSettings;

		public GeneExtension_Undead Giver => def.GetModExtension<GeneExtension_Undead>();

		//[Unsaved(false)]
		private List<HediffDef> cachedPreventiveHediffs;

        public bool PawnCanResurrect
		{
            get
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
        }

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

		//private bool PawnCanResurrect()
		//{
		//	if (Giver.ignoreHediffs)
		//	{
		//		return base.Active;
		//	}
		//	if (!HediffUtility.HasAnyHediff(PreventResurrectionHediffs, pawn))
		//	{
		//		return base.Active;
		//	}
		//	return false;
		//}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, 
			"WVC_XaG_Gene_DisplayStats_Undead_CanResurrect".Translate().CapitalizeFirst(),
			PawnCanResurrect.ToString(), 
			"WVC_XaG_Gene_DisplayStats_Undead_CanResurrect_Desc".Translate()
			+ "\n\n"
			+ (PreventResurrectionHediffs != null ? ("WVC_XaG_Gene_DisplayStats_Undead_CanResurrectHediffs_Desc".Translate() + ":\n"
			+  PreventResurrectionHediffs.Select((HediffDef x) => x.label).ToLineList("  - ", capitalizeItems: true)) : "WVC_XaG_Gene_DisplayStats_Undead_AlwaysResurrect_Desc".Translate()),
			1100);
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			SetCorpse(PawnCanResurrect, Giver.additionalDelay.RandomInRange);
		}

		public void SafeWorkSettings()
		{
			if (pawn.workSettings == null)
			{
				return;
			}
			workSettings = new();
			foreach (WorkTypeDef item in DefDatabase<WorkTypeDef>.AllDefsListForReading)
			{
				workSettings[item] = pawn.workSettings.GetPriority(item);
			}
		}

		public void SetWorkSettings()
		{
			if (pawn.workSettings == null || workSettings.NullOrEmpty())
			{
				return;
			}
			foreach (var item in workSettings)
			{
				pawn.workSettings.SetPriority(item.Key, item.Value);
			}
			workSettings = null;
		}

		public void SetCorpse(bool resurrect, int delay)
		{
			// if (pawn.Corpse == null)
			// {
				// return;
			// }
			//CompHumanlike corpseComp = pawn.TryGetComp<CompHumanlike>();
			//if (corpseComp != null)
			//{
			//	corpseComp.SetUndead(resurrect, delay, pawn);
			//}
			pawn.HumanComponent()?.SetUndead(resurrect, delay, pawn);
		}

		public void Notify_PawnKilled()
		{
			SafeWorkSettings();
		}

		//public void Notify_PawnResurrected()
		//{
		//	SetWorkSettings();
		//}

		// public override void PostAdd()
		// {
		// base.PostAdd();
		// SetCorpse(PawnCanResurrect(), Giver.additionalDelay.RandomInRange);
		// }

		public override void PostRemove()
		{
			base.PostRemove();
			SetCorpse(false, 0);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref workSettings, "workSettings", LookMode.Def, LookMode.Value);
		}

		// public string GetInspectInfo
		// {
		// get
		// {
		// if (PawnCanResurrect)
		// {
		// return "WVC_XaG_Gene_Undead_On_Info".Translate().Resolve();
		// }
		// return null;
		// }
		// }

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
            if (WVC_Biotech.settings.reincarnation_Chance <= 0f)
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
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), ReincarnationActive().ToStringYesNo(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(WVC_Biotech.settings.reincarnation_MinChronoAge.ToString()), 1090);
		}

	}

	public class Gene_Cellular : Gene_AddOrRemoveHediff, IGeneFloatMenuOptions
	{

		public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

		public GeneExtension_Giver Giver => def.GetModExtension<GeneExtension_Giver>();

		// For future sub-genes
		// [Unsaved(false)]
		// private float? cachedRegeneration;

		// public float Regeneration
		// {
			// get
			// {
				// if (!cachedRegeneration.HasValue)
				// {
					// cachedRegeneration = WVC_Biotech.settings.shapeshifer_GeneCellularRegeneration;
				// }
				// return cachedRegeneration.Value;
			// }
		// }

		//private bool? regenerateEyes;
		//public bool RegenerateEyes
		//{
		//	get
		//	{
		//		if (!regenerateEyes.HasValue)
		//		{
		//			regenerateEyes = HealingUtility.ShouldRegenerateEyes(pawn);
		//		}
		//		return regenerateEyes.Value;
		//	}
		//}

        public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(22222, delta))
			{
				return;
			}
			HealingUtility.Regeneration(pawn, regeneration: WVC_Biotech.settings.shapeshifer_GeneCellularRegeneration, tick: 22222);
		}

		public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (!pawn.Downed)
			{
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_XaG_CellularDestroyBody".Translate(), delegate
			{
				Job job = JobMaker.MakeJob(Giver.jobDef, pawn);
				selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			}), selPawn, pawn);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo item in base.GetGizmos())
			{
				yield return item;
			}
			if (!pawn.Downed)
			{
				yield break;
			}
			// if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
			// {
				// yield break;
			// }
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_CellularDestroyBody".Translate(),
				defaultDesc = "WVC_XaG_CellularDestroyBodyDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Pawn> list2 = pawn.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
					for (int i = 0; i < list2.Count; i++)
					{
						Pawn absorber = list2[i];
						if (absorber != pawn && absorber.IsColonist && absorber.CanReach(pawn, PathEndMode.Touch, Danger.Deadly))
						{
							list.Add(new FloatMenuOption(absorber.LabelShort, delegate
							{
								Job job = JobMaker.MakeJob(Giver.jobDef, pawn);
								absorber.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
							}, absorber, Color.white));
						}
					}
					if (!list.Any())
					{
						list.Add(new FloatMenuOption("WVC_XaG_NoSuitableTargets".Translate(), null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			};
		}

		public void ExtractShard(Pawn killer)
		{
			if (ModLister.CheckAnomaly("Shard"))
			{
				if (pawn.SpawnedOrAnyParentSpawned && GenDrop.TryDropSpawn(ThingMaker.MakeThing(ThingDefOf.Shard), pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near, out var resultingThing))
				{
					resultingThing.SetForbidden(!resultingThing.MapHeld.areaManager.Home[resultingThing.PositionHeld]);
					Messages.Message("MessageShardDropped".Translate(pawn.LabelShort).CapitalizeFirst(), resultingThing, MessageTypeDefOf.NeutralEvent);
				}
			}
			ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
			pawn.Kill(new(DamageDefOf.ExecutionCut, 99999, 9999, instigator: killer));
		}

    }

}
