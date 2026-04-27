using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using WVC_XenotypesAndGenes.HarmonyPatches;
// using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Undead : XaG_Gene, IGeneNotifyOnKilled
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

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameNotStarted() && Rand.Chance(0.35f))
			{
				pawn?.HumanComponent()?.SetResurrected();
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
			+ PreventResurrectionHediffs.Select((HediffDef x) => x.label).ToLineList("  - ", capitalizeItems: true)) : "WVC_XaG_Gene_DisplayStats_Undead_AlwaysResurrect_Desc".Translate()),
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

	public class Gene_Reincarnation : XaG_Gene
	{

		public GeneExtension_Undead Spawner => def.GetModExtension<GeneExtension_Undead>();

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Activated())
			{
				return;
			}
			try
			{
				if (Rand.Chance(WVC_Biotech.settings.reincarnation_Chance))
				{
					SetupReincarnation();
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed setup reincarnation quest. Reason: " + arg.Message);
			}
		}

		private void SetupReincarnation()
		{
			XaG_GameComponent xaG_GameComponent = Current.Game?.GetComponent<XaG_GameComponent>();
			if (xaG_GameComponent != null)
			{
				if (xaG_GameComponent.reincarnations == null)
				{
					xaG_GameComponent.reincarnations = new();
				}
				ReincarnationSet newSet = new ReincarnationSet(pawn, Spawner.summonQuest, GestationUtility.GetLitterSize(pawn));
				xaG_GameComponent.reincarnations.Add(newSet);
			}
			//if (XaG_GameComponent.reincarnations == null)
			//{
			//	XaG_GameComponent.reincarnations = new();
			//}
			//XaG_GameComponent.reincarnations.Add(new(pawn, Spawner.summonQuest, GestationUtility.BabiesCount(pawn)));
		}

		private bool Activated()
		{
			if (WVC_Biotech.settings.reincarnation_Chance <= 0f)
			{
				return false;
			}
			if (pawn.Faction != Faction.OfPlayer || pawn.ageTracker.AgeChronologicalYears < WVC_Biotech.settings.reincarnation_MinChronoAge)
			{
				return false;
			}
			if (!Active)
			{
				return false;
			}
			return true;
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate".Translate().CapitalizeFirst(), Activated().ToStringYesNo(), "WVC_XaG_Gene_DisplayStats_Undead_CanReincarnate_Desc".Translate(WVC_Biotech.settings.reincarnation_MinChronoAge.ToString()), 1090);
		}

	}

	public class Gene_Cellular : Gene_AddOrRemoveHediff, IGeneFloatMenuOptions
	{

		//public GeneExtension_Undead Undead => def.GetModExtension<GeneExtension_Undead>();

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
			base.TickInterval(22222);
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
			//foreach (Gizmo item in base.GetGizmos())
			//{
			//	yield return item;
			//}
			if (!pawn.Downed)
			{
				yield break;
			}
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

		//public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		//{
		//	if (!Active)
		//	{
		//		return;
		//	}
		//	HealingUtility.DisableRot(pawn);
		//}

		//=================

		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	HarmonyPatch();
		//	ResetCache();
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	ResetCache();
		//}

		//public override void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	base.Notify_OverriddenBy(overriddenBy);
		//	ResetCache();
		//}

		//public override void Notify_Override()
		//{
		//	base.Notify_Override();
		//	HarmonyPatch();
		//	ResetCache();
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	if (Scribe.mode == LoadSaveMode.PostLoadInit)
		//	{
		//		HarmonyPatch();
		//	}
		//}

		//private static void ResetCache()
		//{
		//	DeathlessUtility.ResetCollection();
		//}

		//private static void HarmonyPatch()
		//{
		//	DeathlessUtility.HarmonyPatch();
		//}

	}

	public class Gene_Obsession : XaG_Gene
	{

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active || pawn.ageTracker?.Adult != true)
			{
				return;
			}
			try
			{
				DoAction();
			}
			catch (Exception arg)
			{
				Log.Error("Failed trigger obsession. For gene: " + def.defName + ". Reason: " + arg.Message);
			}
		}

		private void DoAction()
		{
			if (pawn.Faction != Faction.OfPlayer || pawn.relations == null || pawn.relations.ChildrenCount <= 0 || pawn.skills == null || pawn.story?.traits == null || !pawn.IsPsychicSensitive())
			{
				return;
			}
			List<Pawn> children = pawn.relations.Children.ToList();
			children.SortBy(child => child.ageTracker.Adult);
			foreach (Pawn child in children)
			{
				if (TryObsession(child))
				{
					child.needs?.mood?.thoughts?.memories?.RemoveMemoriesWhereOtherPawnIs(pawn);
					break;
				}
			}
		}

		private bool TryObsession(Pawn child)
		{
			if (child.ageTracker.CurLifeStage.alwaysDowned || !child.IsHuman() || child.skills == null || !child.IsPsychicSensitive() || child.Map == null || child.Faction != pawn.Faction)
			{
				return false;
			}
			if (child.ageTracker.Adult)
			{
				child.story.Adulthood = pawn.story.Adulthood;
				child.story.Childhood = pawn.story.Childhood;
				TransferSkills(child);
				foreach (Trait trait in child.story.traits.allTraits.ToList())
				{
					if (trait.sourceGene != null)
					{
						continue;
					}
					trait.RemoveTrait(child);
				}
				foreach (Trait trait in pawn.story.traits.allTraits.ToList())
				{
					if (trait.sourceGene != null)
					{
						continue;
					}
					if (child.CanGetTrait(trait.def))
					{
						Trait newTrait = new Trait(trait.def, trait.Degree);
						child.story.traits.GainTrait(newTrait);
					}
				}
			}
			else
			{
				TransferSkills(child);
			}
			return false;

			void TransferSkills(Pawn child)
			{
				foreach (SkillRecord parentSkill in pawn.skills.skills)
				{
					foreach (SkillRecord childSkill in child.skills.skills)
					{
						if (parentSkill.def == childSkill.def)
						{
							childSkill.Level = parentSkill.Level;
							childSkill.xpSinceLastLevel = parentSkill.xpSinceLastLevel;
							childSkill.passion = parentSkill.passion;
						}
					}
				}
			}
		}

	}

}
