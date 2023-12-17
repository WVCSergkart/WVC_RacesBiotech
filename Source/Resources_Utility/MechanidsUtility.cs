using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld.QuestGen;

namespace WVC_XenotypesAndGenes
{

	public static class MechanoidsUtility
	{

		public static void MechSummonQuest(Pawn pawn, QuestScriptDef quest)
		{
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
			float weight = mechanitor.mechanitor.TotalBandwidth - (mechanitor.mechanitor.UsedBandwidth + mech.GetStatValue(StatDefOf.BandwidthCost));
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
			if (!ModsConfig.BiotechActive)
			{
				return;
			}
			// Log.Error("2");
			Need_MechEnergy energy = pawn?.needs?.energy;
			if (energy != null && (energy.IsSelfShutdown || energy.IsLowEnergySelfShutdown))
			{
				energy.CurLevel += offset * hours;
				// Log.Error("3: " + energy.CurLevel);
			}
		}

		public static bool MechanoidIsGolemlike(Pawn mech)
		{
			if (WalkingUtility.PawnIsBoneGolem(mech) || GolemsUtility.PawnIsGolem(mech))
			{
				return true;
			}
			return false;
		}

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
