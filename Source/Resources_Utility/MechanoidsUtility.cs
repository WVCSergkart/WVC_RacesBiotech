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

		public static List<Pawn> GetAllControlledGolems(Pawn mechanitor)
		{
			List<Pawn> mechs = mechanitor?.mechanitor?.ControlledPawns;
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
				if (item.IsGolemlike() && !item.health.Dead)
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
				return pawn.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost) > 0;
			}
			return false;
		}

		public static float TotalGolembond(Pawn mechanitor)
		{
			return mechanitor.GetStatValue(WVC_GenesDefOf.WVC_GolemBond);
		}

		public static float GetConsumedGolembond(Pawn mechanitor)
		{
			float result = 0;
			List <Pawn> golems = mechanitor?.mechanitor?.ControlledPawns;
			if (golems.NullOrEmpty())
			{
				return result;
			}
			foreach (Pawn golem in golems)
			{
				result += golem.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost);
			}
			return result;
		}

		public static bool MechanitorIsLich(Pawn mechanitor)
		{
			if (!MechanitorUtility.IsMechanitor(mechanitor))
			{
				return false;
			}
			// Log.Error("TEST is Golem Master");
			Gene_MechlinkWithGizmo gene = mechanitor.genes?.GetFirstGeneOfType<Gene_MechlinkWithGizmo>();
			if (gene?.def?.resourceGizmoType == typeof(GeneGizmo_ResurgentSpores))
			{
				return true;
			}
			return false;
		}

		public static bool MechanitorIsGolemist(Pawn mechanitor)
		{
			if (!MechanitorUtility.IsMechanitor(mechanitor))
			{
				return false;
			}
			// Log.Error("TEST is Golem Master");
			Gene_MechlinkWithGizmo gene = mechanitor.genes?.GetFirstGeneOfType<Gene_MechlinkWithGizmo>();
			if (gene?.def?.resourceGizmoType == typeof(Gizmo_MaxGolems))
			{
				return true;
			}
			return false;
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
