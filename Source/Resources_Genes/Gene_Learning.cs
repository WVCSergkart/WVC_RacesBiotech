using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_BackstoryChanger : Gene
	{

		// public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>().childBackstoryDef;
		// public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>().adultBackstoryDef;
		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			if (Props != null)
			{
				BackstoryChanger(pawn, Props.childBackstoryDef, Props.adultBackstoryDef);
			}
		}

		public override void TickInterval(int delta)
		{

		}

		public static void BackstoryChanger(Pawn pawn, BackstoryDef childBackstoryDef = null, BackstoryDef adultBackstoryDef = null)
		{
			if ((pawn?.story) == null)
			{
				return;
			}
			if (pawn.story.Childhood == null)
			{
				return;
			}
			if (pawn.Spawned)
			{
				return;
			}
			List<BackstoryDef> blackListedBackstoryForChanger = ListsUtility.GetBlackListedBackstoryForChanger();
			if (childBackstoryDef != null && !blackListedBackstoryForChanger.Contains(pawn.story.Childhood))
			{
				pawn.story.Childhood = childBackstoryDef;
			}
			if (pawn.story.Adulthood != null && adultBackstoryDef != null && !blackListedBackstoryForChanger.Contains(pawn.story.Adulthood))
			{
				pawn.story.Adulthood = adultBackstoryDef;
			}
		}

	}

	public class Gene_Learning : Gene_BackstoryChanger
	{

		public GeneExtension_General General => def?.GetModExtension<GeneExtension_General>();

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(3000, delta))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			UnDecaySkills(pawn, 100, 0);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Flat skills exp",
					action = delegate
					{
						FlatAllSkillsExp(pawn);
					}
				};
			}
		}

		public void UnDecaySkills(Pawn pawn, int reserve = 500, int minExp = 0)
		{
			foreach (SkillRecord skill in pawn.skills.skills)
			{
				// if (skill.TotallyDisabled || skill.PermanentlyDisabled)
				// {
					// continue;
				// }
				int reservedExp = reserve * skill.GetLevel(false);
				if (skill.xpSinceLastLevel - reservedExp > minExp)
				{
					continue;
				}
				skill.xpSinceLastLevel = (float)reservedExp;
			}
		}

		public void FlatAllSkillsExp(Pawn pawn)
		{
			foreach (SkillRecord skill in pawn.skills.skills)
			{
				// if (skill.TotallyDisabled || skill.PermanentlyDisabled)
				// {
					// continue;
				// }
				skill.xpSinceLastLevel = 0f;
			}
		}

	}

	public class Gene_LearningTelepath : Gene_BackstoryChanger
	{

		private int nextTick = 6000;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			nextTick -= delta;
			if (nextTick > 0)
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			TryLearning();
			ResetInterval();
		}

		public void TryLearning()
		{
			GeneFeaturesUtility.TryLearning(pawn, 0.2f, WVC_Biotech.settings.learningTelepathWorkForBothSides, 1);
		}

		private void ResetInterval()
		{
			IntRange range = new(42000, 90000);
			nextTick = range.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Learn skills from colony",
					action = delegate
					{
						TryLearning();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "hashIntervalTick", 6000);
		}

	}

	public class Gene_Blank : Gene
	{

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(200, delta))
			{
				return;
			}
			SkillLose();
		}

		public void SkillLose()
		{
			foreach (SkillRecord skill in pawn.skills.skills)
			{
				skill.Learn(-1f * skill.GetLevel(false), true);
			}
		}

	}

	public class Gene_PsychicNetwork : Gene_Speaker
	{

		//public string RemoteActionName
		//{
		//	get
		//	{
		//		if (isActive)
		//		{
		//			return "WVC_XaG_Gene_DustMechlink_On".Translate();
		//		}
		//		return "WVC_XaG_Gene_DustMechlink_Off".Translate();
		//	}
		//}

  //      public string RemoteActionDesc => "WVC_XaG_RemoteControlEnergyDesc".Translate();

  //      public override bool Active
		//{
		//	get
		//	{
		//		if (!isActive)
		//		{
		//			return false;
		//		}
		//		return base.Active;
		//	}
		//}

		//private bool isActive = true;

		//public void Remote—ontrol()
		//{
		//	isActive = !isActive;
		//	XaG_GeneUtility.Notify_GenesChanged(pawn);
		//}

		private int currentRange = 0;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval(new(6000, 90000));
		}

		public override void TryInteract()
		{
			if (currentRange < 30000 || !TryLearning(pawn, 0.10f))
			{
				GeneInteractionsUtility.TryInteractRandomly(pawn, true, true, false, out _, this);
			}
			else
			{
				FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			}
			ResetInterval(new(6000, 90000));
		}

		public bool TryLearning(Pawn pawn, float learnPercent = 0.1f)
		{
			if (pawn?.Map == null || pawn.Downed)
			{
				return false;
			}
			List<Pawn> workingList = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
			workingList.Shuffle();
			for (int i = 0; i < workingList.Count; i++)
			{
				Pawn p = workingList[i];
				if (!p.RaceProps.Humanlike)
				{
					continue;
				}
				if (!p.IsPsychicSensitive())
				{
					continue;
				}
				if (p == pawn)
				{
					continue;
				}
				// Log.Error("Try find pawn with same gene");
				if (!XaG_GeneUtility.HasGeneOfType(this, p))
				{
					continue;
				}
				// Log.Error(p.Name.ToString());
				return GeneFeaturesUtility.TryGetRandomSkillFromPawn(pawn, p, learnPercent);
			}
			return true;
		}

		public override void ResetInterval(IntRange range)
		{
			currentRange = range.RandomInRange;
			base.ResetInterval(new(currentRange, currentRange));
		}

		public override void ExposeData()
		{
			base.ExposeData();
			//Scribe_Values.Look(ref isActive, "isActive", defaultValue: true);
			Scribe_Values.Look(ref currentRange, "currentRange", 0);
		}

	}

	public class Gene_CyclicallySelfLearning : Gene
	{

		// private int hashIntervalTick = 120000;

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// ResetInterval();
		// }

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(119357, delta))
			{
				return;
			}
			// ResetInterval();
			TryLearning();
		}

		public void TryLearning()
		{
			GeneFeaturesUtility.TryLevelUpRandomSkill(pawn, (int)WVC_Biotech.settings.learning_CyclicallySelfLearning_MaxSkillLevel);
		}

		// private void ResetInterval()
		// {
			//IntRange range = new(120000, 240000);
			//hashIntervalTick = range.RandomInRange;
			// hashIntervalTick = 120000;
		// }

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryLevelUpRandomSkill",
					action = delegate
					{
						TryLearning();
					}
				};
			}
		}

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 120000);
		// }

	}

}
