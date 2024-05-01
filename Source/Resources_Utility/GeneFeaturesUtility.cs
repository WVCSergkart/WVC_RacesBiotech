using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class GeneFeaturesUtility
	{

		// ============================= GENE THRALL =============================

		public static bool CanCellsFeedNowWith(Pawn biter, Pawn victim)
		{
			if (MiscUtility.BasicTargetValidation(biter, victim))
			{
				Gene_ResurgentCells cells = victim.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
				if (cells == null)
				{
					return false;
				}
				if (cells.ValuePercent < 0.20f)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static void DoCellsBite(Pawn biter, Pawn victim, float daysGain, float cellsConsumeFactor, float nutritionGain, IntRange bloodFilthToSpawnRange, float targetBloodLoss = 0.03f)
		{
			float cells = daysGain * cellsConsumeFactor;
			int ticks = (int)(daysGain * (victim.BodySize * 60000));
			XaG_GeneUtility.OffsetInstabilityTick(biter, ticks);
			UndeadUtility.OffsetResurgentCells(victim, 0f - (cells * 0.01f));
			if (biter.needs?.food != null)
			{
				biter.needs.food.CurLevel += nutritionGain * cells;
			}
			if (!victim.WouldDieFromAdditionalBloodLoss(targetBloodLoss) && targetBloodLoss > 0f)
			{
				victim.health.AddHediff(HediffDefOf.BloodfeederMark, ExecutionUtility.ExecuteCutPart(victim));
				Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, victim);
				hediff.Severity = targetBloodLoss;
				victim.health.AddHediff(hediff);
			}
			TrySpawnBloodFilth(victim, bloodFilthToSpawnRange);
		}

		public static bool TrySpawnBloodFilth(Pawn victim, IntRange bloodFilthToSpawnRange)
		{
			if (victim?.Map == null)
			{
				return false;
			}
			int randomInRange = bloodFilthToSpawnRange.RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				IntVec3 c = victim.Position;
				if (randomInRange > 1 && Rand.Chance(0.8888f))
				{
					c = victim.Position.RandomAdjacentCell8Way();
				}
				if (c.InBounds(victim.MapHeld))
				{
					FilthMaker.TryMakeFilth(c, victim.MapHeld, victim.RaceProps.BloodDef, victim.LabelShort);
				}
			}
			return true;
		}

		// ============================= GENE Learning Telepath =============================

		public static bool TryLearning(Pawn pawn, float learnPercent = 0.2f, bool shareSkills = false)
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
				// if (GeneFeaturesUtility.CanPsyFeedNowWith(pawn, p))
				// {
				// }
				TryGetSkillsFromPawn(pawn, p, learnPercent, shareSkills);
			}
			FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			return true;
		}

		public static void TryGetSkillsFromPawn(Pawn student, Pawn teacher, float learnPercent, bool shareSkills = false)
		{
			List<SkillRecord> teacherSkills = teacher?.skills?.skills;
			if (teacherSkills == null || student?.skills?.skills == null)
			{
				return;
			}
			foreach (SkillRecord skill in student.skills.skills)
			{
				if (skill.TotallyDisabled || skill.PermanentlyDisabled)
				{
					continue;
				}
				// Log.Error(skill.def.LabelCap + " try learn");
				foreach (SkillRecord teacherSkill in teacherSkills)
				{
					if (skill.def != teacherSkill.def)
					{
						continue;
					}
					if (teacherSkill.TotallyDisabled || teacherSkill.PermanentlyDisabled)
					{
						break;
					}
					if (teacherSkill.GetLevel(false) < skill.GetLevel(false))
					{
						if (shareSkills)
						{
							teacherSkill.Learn(skill.XpTotalEarned * learnPercent, true);
							// Log.Error(skill.def.LabelCap + " teached exp " + (teacherSkill.XpTotalEarned * learnPercent).ToString());
						}
						break;
					}
					if (teacherSkill.GetLevel(false) > skill.GetLevel())
					{
						skill.Learn(teacherSkill.XpTotalEarned * learnPercent, true);
						// Log.Error(skill.def.LabelCap + " learned exp " + (teacherSkill.XpTotalEarned * learnPercent).ToString());
					}
					break;
				}
			}
		}

		// ============================= GENE PSY HARVESTER =============================

		public static bool TryHarvest(Pawn pawn, ThingDef thingDef, int stackCount, float targetBloodLoss = 0.4499f, ThingStyleDef styleDef = null)
		{
			if (pawn?.Map == null || pawn.Downed)
			{
				return false;
			}
			List<Pawn> workingList = MiscUtility.GetAllPlayerControlledMapPawns_ForBloodfeed(pawn);
			// workingList.Shuffle();
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
				if (CanBloodFeedNowWith(pawn, p))
				{
					DoPsychicHarvest(pawn, p, thingDef, stackCount, targetBloodLoss, new (1, 2), styleDef: styleDef);
				}
			}
			FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
			return true;
		}

		public static void DoPsychicHarvest(Pawn biter, Pawn victim, ThingDef thingDef, int stackCount, float targetBloodLoss, IntRange bloodFilthToSpawnRange, ThingStyleDef styleDef = null)
		{
			if (victim?.Map == null || biter?.Map == null)
			{
				return;
			}
			float num = SanguophageUtility.HemogenGainBloodlossFactor(victim, targetBloodLoss);
			int finalStack = (int)(stackCount * victim.BodySize * num);
			// if (finalStack <= 0)
			// {
				// return;
			// }
			Gene_BloodyGrowths.SpawnItems(victim, thingDef, finalStack > 1 ? finalStack : 1, styleDef: styleDef);
			if (targetBloodLoss > 0f)
			{
				Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, victim);
				hediff.Severity = targetBloodLoss;
				victim.health.AddHediff(hediff);
				SoundDefOf.Execute_Cut.PlayOneShot(victim);
				TrySpawnBloodFilth(victim, bloodFilthToSpawnRange);
			}
		}

		// ============================= GENE PSY BLOODFEEDER =============================

		public static bool TryPsyFeedRandomly(Pawn pawn, Gene_Hemogen gene_Hemogen)
		{
			if (pawn?.Map == null || gene_Hemogen == null || pawn.Downed)
			{
				return false;
			}
			if (gene_Hemogen.hemogenPacksAllowed || !gene_Hemogen.ShouldConsumeHemogenNow())
			{
				return false;
			}
			List<Pawn> workingList = MiscUtility.GetAllPlayerControlledMapPawns_ForBloodfeed(pawn);
			// workingList.Shuffle();
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
				if (CanBloodFeedNowWith(pawn, p))
				{
					return TryPsychicBite(pawn, p);
				}
			}
			return false;
		}

		public static bool CanBloodFeedNowWith(Pawn biter, Pawn victim)
		{
			if (MiscUtility.BasicTargetValidation(biter, victim))
			{
				// if (victim.guest?.IsInteractionDisabled(PrisonerInteractionModeDefOf.Bloodfeed) == true)
				// {
					// return false;
				// }
				if (victim.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
				{
					return false;
				}
				if (victim.genes?.GetFirstGeneOfType<Gene_Hemogen>() != null)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static bool TryPsychicBite(Pawn biter, Pawn victim)
		{
			Ability ability = biter.abilities?.GetAbility(WVC_GenesDefOf.Bloodfeed);
			if (ability == null)
			{
				return false;
			}
			LocalTargetInfo target = victim;
			if (ability.Activate(target, null))
			{
				SoundDefOf.Execute_Cut.PlayOneShot(victim);
				FleckMaker.AttachedOverlay(biter, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
				return true;
			}
			return false;
		}

		// public static void DoPsychicBite(Pawn biter, Pawn victim, float nutritionGain, float targetHemogenGain, float targetBloodLoss, IntRange bloodFilthToSpawnRange)
		// {
			// float num = SanguophageUtility.HemogenGainBloodlossFactor(victim, targetBloodLoss);
			// float num2 = targetHemogenGain * victim.BodySize * num;
			// GeneUtility.OffsetHemogen(biter, num2);
			// GeneUtility.OffsetHemogen(victim, 0f - num2);
			// if (biter.needs?.food != null)
			// {
				// biter.needs.food.CurLevel += nutritionGain * num;
			// }
			// if (targetBloodLoss > 0f)
			// {
				// Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, victim);
				// hediff.Severity = targetBloodLoss;
				// victim.health.AddHediff(hediff);
			// }
			// if (victim.Map == null)
			// {
				// return;
			// }
			// SoundDefOf.Execute_Cut.PlayOneShot(victim);
			// int randomInRange = bloodFilthToSpawnRange.RandomInRange;
			// for (int i = 0; i < randomInRange; i++)
			// {
				// IntVec3 c = victim.Position;
				// if (randomInRange > 1 && Rand.Chance(0.8888f))
				// {
					// c = victim.Position.RandomAdjacentCell8Way();
				// }
				// if (c.InBounds(victim.MapHeld))
				// {
					// FilthMaker.TryMakeFilth(c, victim.MapHeld, victim.RaceProps.BloodDef, victim.LabelShort);
				// }
			// }
			// FleckMaker.AttachedOverlay(biter, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
		// }

		// ============================= Checker Gene Features =============================

		public static bool IsNotAcceptablePrey(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true)
				{
					GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
					if (modExtension != null)
					{
						if (!modExtension.canBePredatorPrey)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool PawnSkillsNotDecay(Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Learning>() != null;
		}

		public static bool EyesShouldBeInvisble(Pawn pawn)
		{
			if (pawn?.genes == null || pawn?.story == null || pawn?.story?.headType == null)
			{
				return false;
			}
			if (pawn.story.headType.defName.Contains("WVC_Faceless"))
			{
				return true;
			}
			return false;
		}


	}
}
