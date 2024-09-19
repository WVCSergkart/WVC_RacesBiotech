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
			List<BackstoryDef> blackListedBackstoryForChanger = XenotypeFilterUtility.BlackListedBackstoryForChanger();
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

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(3000))
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

		private int hashIntervalTick = 6000;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(hashIntervalTick))
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
			hashIntervalTick = range.RandomInRange;
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
			Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 6000);
		}

	}

	public class Gene_Blank : Gene
	{

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(200))
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

	public class Gene_PsychicNetwork : Gene
	{

		private int hashIntervalTick = 6000;

		private int currentRange = 0;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			// base.Tick();
			if (!pawn.IsHashIntervalTick(hashIntervalTick))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			TryInteractOrLearning();
			ResetInterval();
		}

		public void TryInteractOrLearning()
		{
			if (currentRange < 30000)
			{
				ThoughtUtility.TryInteractRandomly(pawn, this);
			}
			else
			{
				TryLearning(pawn, 0.05f);
			}
			// Log.Error(currentRange.ToString());
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
				if (!p.PawnPsychicSensitive())
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
				GeneFeaturesUtility.TryGetRandomSkillFromPawn(pawn, p, learnPercent);
				break;
			}
			FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			return true;
		}

		private void ResetInterval()
		{
			IntRange range = new(6000, 90000);
			currentRange = range.RandomInRange;
			hashIntervalTick = currentRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryInteractOrLearning",
					action = delegate
					{
						TryInteractOrLearning();
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 6000);
			Scribe_Values.Look(ref currentRange, "currentRange", 0);
		}

	}

	public class Gene_CyclicallySelfLearning : Gene
	{

		private int hashIntervalTick = 6000;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(hashIntervalTick))
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
			GeneFeaturesUtility.TryLevelUpRandomSkill(pawn);
		}

		private void ResetInterval()
		{
			IntRange range = new(60000, 120000);
			hashIntervalTick = range.RandomInRange;
		}

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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 6000);
		}

	}

}
