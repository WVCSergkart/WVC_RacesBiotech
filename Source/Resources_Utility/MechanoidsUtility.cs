using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class MechanoidsUtility
	{

		// Golems

		public static bool HasEnoughGolembond(Pawn mechanitor)
		{
			return TotalGolembond(mechanitor) >= GetConsumedGolembond(mechanitor);
		}

		public static List<Pawn> GetAllControlledGolems(Pawn mechanitor)
		{
			List<Pawn> mechs = mechanitor?.mechanitor?.OverseenPawns;
			if (mechs.NullOrEmpty())
			{
				return null;
			}
			List<Pawn> list = new();
			foreach (Pawn item in mechs)
			{
				if (item.health.Dead)
				{
					continue;
				}
				if (!item.IsGolemlike())
				{
					continue;
				}
				// if (compGolem.Props.golemIndex == golemIndex)
				// {
				// }
				list.Add(item);
			}
			return list;
		}

		[Obsolete]
		public static List<Pawn> GetAllControlledGolemsOfIndex(Pawn mechanitor, int golemIndex)
		{
			List<Pawn> mechs = mechanitor?.mechanitor?.OverseenPawns;
			if (mechs.NullOrEmpty())
			{
				return null;
			}
			List<Pawn> list = new();
			foreach (Pawn item in mechs)
			{
				if (item.health.Dead)
				{
					continue;
				}
				if (!item.IsGolemlike())
				{
					continue;
				}
				CompGolem compGolem = item.GetComp<CompGolem>();
				if (compGolem == null)
				{
					continue;
				}
				if (compGolem.Props.golemIndex == golemIndex)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static bool IsGolemlike(this Pawn pawn)
		{
			if (pawn.RaceProps.IsMechanoid)
			{
				return pawn.TryGetComp<CompGolem>() != null;
			}
			return false;
		}

		public static bool IsGolemlike(this PawnKindDef pawnkind)
		{
			if (pawnkind.race.race.IsMechanoid)
			{
				return pawnkind.race.GetCompProperties<CompProperties_Golem>() != null;
			}
			return false;
		}

		// public static bool GetGolemistGene(this Pawn pawn, out Gene_MechlinkWithGizmo mechlinkWithGizmo)
		// {
			// return (mechlinkWithGizmo = pawn?.genes?.GetFirstGeneOfType<Gene_MechlinkWithGizmo>())?.def?.resourceGizmoType == typeof(GeneGizmo_Golems);
		// }


		public static float TotalGolembond(Pawn mechanitor)
		{
			return mechanitor.GetStatValue(WVC_GenesDefOf.WVC_GolemBond, cacheStaleAfterTicks: 120000);
		}

		public static float GetConsumedGolembond(Pawn mechanitor)
		{
			float result = 0;
			List <Pawn> golems = mechanitor?.mechanitor?.OverseenPawns;
			if (golems.NullOrEmpty())
			{
				return result;
			}
			foreach (Pawn golem in golems)
			{
				if (golem.health.Dead)
				{
					continue;
				}
				result += golem.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost, cacheStaleAfterTicks: 360000);
			}
			return result;
		}

		// public static bool IsGolemistOfIndex(this Pawn mechanitor, int golemsIndex)
		// {
			// if (MechanitorUtility.IsMechanitor(mechanitor))
			// {
				// return mechanitor.GetGolemistGene(out Gene_MechlinkWithGizmo mechlinkWithGizmo) && mechlinkWithGizmo?.Giver?.golemistTypeIndex == golemsIndex;
			// }
			// return false;
		// }

		[Obsolete]
		public static bool MechanitorIsLich(Pawn mechanitor)
		{
			return mechanitor?.genes?.GetFirstGeneOfType<Gene_Sporelink>() != null;
		}

		public static bool MechanitorIsGolemist(Pawn mechanitor)
		{
			return mechanitor?.genes?.GetFirstGeneOfType<Gene_Golemlink>() != null;
		}

		// Ideo

		// public static bool CanSummonMechanoidsIdeo(Pawn pawn)
		// {
			// if (!ModLister.CheckIdeology("Ideology"))
			// {
				// return true;
			// }
			// List<Precept> preceptsListForReading = pawn?.ideo?.Ideo?.PreceptsListForReading;
			// if (preceptsListForReading.NullOrEmpty())
			// {
				// return true;
			// }
			// foreach (Precept precept in preceptsListForReading)
			// {
				// PreceptExtension_General preceptExtension = precept?.def?.GetModExtension<PreceptExtension_General>();
				// if (preceptExtension != null)
				// {
					// return !preceptExtension.blesslinkCannotSummonMechanoids;
				// }
			// }
			// return true;
		// }

		public static void MechSummonQuest(Pawn pawn, QuestScriptDef quest)
		{
			if (pawn.Map.IsUnderground())
			{
				return;
			}
			Slate slate = new();
			slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(pawn.Map));
			slate.Set("asker", pawn);
			_ = QuestUtility.GenerateQuestAndMakeAvailable(quest, slate);
			// QuestUtility.SendLetterQuestAvailable(quest);
		}

		// ===============================================================

		// public static bool LimitExceedCheck(Pawn mechanitor, StatDef bandwidth)
		// {
			// float totalBandwidth = mechanitor.GetStatValue(bandwidth);
			// float golems = GolemsUtility.GetConsumedBandwidth(mechanitor);
			// float walkers = WalkingUtility.GetConsumedBandwidth(mechanitor);
			// if (totalBandwidth >= golems && totalBandwidth >= walkers)
			// {
				// return true;
			// }
			// return false;
		// }

		// public static float GetConsumedBandwidth(Pawn mechanitor, StatDef bandwidth)
		// {
			// float result = 0;
			// List <Pawn> mechs = mechanitor.mechanitor.ControlledPawns;
			// if (mechs.NullOrEmpty())
			// {
				// return result;
			// }
			// foreach (Pawn mech in mechs)
			// {
				// float bandwidth = mech.GetStatValue(bandwidth);
				// if (bandwidth > 0)
				// {
					// result += bandwidth;
				// }
			// }
			// return result;
		// }

		// =====================================

		public static List<Pawn> GetAllMechanitors(Map map)
		{
			List<Pawn> list = new();
			List<Pawn> colonists = map.mapPawns.FreeColonists;
			if (!colonists.NullOrEmpty())
			{
				foreach (Pawn colonist in colonists)
				{
					if (MechanitorUtility.IsMechanitor(colonist))
					{
						list.Add(colonist);
					}
				}
			}
			return list;
		}

		public static bool CanSpawnMoreMechanoids(Pawn mechanitor, Pawn mech)
		{
			float weight = mechanitor.mechanitor.TotalBandwidth - (mechanitor.mechanitor.UsedBandwidth + mech.GetStatValue(StatDefOf.BandwidthCost, cacheStaleAfterTicks: 360000));
			if (weight < 0f)
			{
				return false;
			}
			return true;
		}

		// public static float GetConsumedBandwidth(Pawn mechanitor)
		// {
			// float result = 0;
			// List <Pawn> mechs = mechanitor.mechanitor.ControlledPawns;
			// if (mechs.NullOrEmpty())
			// {
				// return result;
			// }
			// foreach (Pawn mech in mechs)
			// {
				// float bandwidth = mech.GetStatValue(StatDefOf.BandwidthCost);
				// if (bandwidth > 0)
				// {
					// result += bandwidth;
				// }
			// }
			// return result;
		// }

		public static void OffsetNeedEnergy(Pawn pawn, float offset, int hours)
		{
			Need_MechEnergy energy = pawn?.needs?.energy;
			if (energy?.IsSelfShutdown == true)
			{
				energy.CurLevel += offset * hours;
			}
		}

		// [Obsolete]
		// public static bool MechanoidIsGolemlike(Pawn mech)
		// {
			// if (WalkingUtility.PawnIsBoneGolem(mech) || GolemsUtility.PawnIsGolem(mech))
			// {
				// return true;
			// }
			// return false;
		// }

		// Mecha summon
		public static bool MechanoidIsPlayerMechanoid(PawnKindDef mech)
		{
			if (mech.race.race.IsMechanoid 
			&& mech.defName.Contains("Mech_") 
			&& MechDefNameShouldNotContain(mech.defName)
			&& MechDefNameShouldNotContain(mech.race.defName)
			&& mech.race.race.thinkTreeMain == WVC_GenesDefOf.Mechanoid 
			&& mech.race.race.thinkTreeConstant == WVC_GenesDefOf.MechConstant 
			// && mech.race.race.maxMechEnergy == 100
			&& mech.race.race.lifeStageAges.Count > 1
			&& EverControllable(mech.race)
			&& EverRepairable(mech.race))
			{
				return true;
			}
			return false;
		}

		public static bool MechDefNameShouldNotContain(string defName)
		{
			List<string> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.mechDefNameShouldNotContain);
			}
			if (list.Contains(defName))
			{
				return false;
			}
			return true;
		}

		public static bool EverControllable(ThingDef def)
		{
			List<CompProperties> comps = def.comps;
			for (int i = 0; i < comps.Count; i++)
			{
				if (comps[i].compClass == typeof(CompOverseerSubject))
				{
					return true;
				}
			}
			return false;
		}

		public static bool EverRepairable(ThingDef def)
		{
			List<CompProperties> comps = def.comps;
			for (int i = 0; i < comps.Count; i++)
			{
				if (comps[i].compClass == typeof(CompMechRepairable))
				{
					return true;
				}
			}
			return false;
		}


	}
}
