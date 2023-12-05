using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class WalkingUtility
	{

		public static bool CanChoseMechPawnKindDef(Pawn lich, PawnKindDef golem)
		{
			float weight = lich.mechanitor.TotalBandwidth - (lich.mechanitor.UsedBandwidth + golem.race.statBases.GetStatValueFromList(StatDefOf.BandwidthCost, 100f));
			if (weight < 0f)
			{
				return false;
			}
			weight = TotalSporesBandwidth(lich) - GetConsumedBandwidth(lich) - golem.race.statBases.GetStatValueFromList(WVC_GenesDefOf.WVC_SporesBandwidthCost, 100f);
			if (weight < 0f)
			{
				return false;
			}
			return true;
		}

		// public static bool AnyLichCanSpawnMoreCorpses(List<Pawn> lichs, Pawn golem)
		// {
			// foreach (Pawn lich in lichs)
			// {
				// return CanSpawnMoreCorpses(lich, golem);
			// }
			// return false;
		// }

		public static bool CanSpawnMoreCorpses(Pawn lich, Pawn golem)
		{
			if (MechanoidsUtility.CanSpawnMoreMechanoids(lich, golem))
			{
				float weight = TotalSporesBandwidth(lich) - GetConsumedBandwidth(lich) - golem.GetStatValue(WVC_GenesDefOf.WVC_SporesBandwidthCost);
				if (weight < 0f)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static float GetLichWeight(Pawn lich)
		{
			float weight = TotalSporesBandwidth(lich) - GetConsumedBandwidth(lich);
			if (weight <= 0f)
			{
				weight = 0.001f;
			}
			return weight;
		}

		public static List<Pawn> GetAllLichs(Map map)
		{
			List<Pawn> list = new();
			List<Pawn> colonists = map.mapPawns.FreeColonists;
			// Log.Error("colonists: " + colonists.Count);
			if (!colonists.NullOrEmpty())
			{
				foreach (Pawn colonist in colonists)
				{
					if (MechanitorIsLich(colonist))
					{
						list.Add(colonist);
					}
				}
			}
			// Log.Error("lichs: " + list.Count);
			return list;
		}

		public static bool MechanitorIsLich(Pawn mechanitor)
		{
			if (!MechanitorUtility.IsMechanitor(mechanitor))
			{
				return false;
			}
			Gene_ResurgentMechlink gene_ResurgentMechlink = mechanitor.genes?.GetFirstGeneOfType<Gene_ResurgentMechlink>();
			if (gene_ResurgentMechlink != null && MechanoidizationUtility.HasActiveGene(gene_ResurgentMechlink.def, mechanitor))
			{
				if (TotalSporesBandwidth(mechanitor) > 0)
				{
					return true;
				}
			}
			return false;
		}

		public static int TotalSporesBandwidth(Pawn mechanitor)
		{
			return (int)mechanitor.GetStatValue(WVC_GenesDefOf.WVC_SporesBandwidth);
		}

		public static bool MechanitorHasAnyWalkingCorpses(Pawn mechanitor)
		{
			List<Pawn> list = GetControlledWalkingCorpses(mechanitor);
			if (list.NullOrEmpty())
			{
				return false;
			}
			return true;
		}

		public static bool LimitExceedCheck(Pawn mechanitor)
		{
			int maxGolems = TotalSporesBandwidth(mechanitor);
			// if (maxGolems <= 0f)
			// {
				// return false;
			// }
			int bandwidthCost = GetConsumedBandwidth(mechanitor);
			if (maxGolems >= bandwidthCost)
			{
				return true;
			}
			return false;
		}

		public static int GetConsumedBandwidth(Pawn mechanitor)
		{
			int result = 0;
			List <Pawn> golems = GetControlledWalkingCorpses(mechanitor);
			if (golems.NullOrEmpty())
			{
				return result;
			}
			foreach (Pawn golem in golems)
			{
				int golemBand = (int)golem.GetStatValue(WVC_GenesDefOf.WVC_SporesBandwidthCost);
				if (golemBand > 0)
				{
					result += golemBand;
				}
			}
			// Log.Error("GetConsumedBandwidth: " + result);
			return result;
		}

		public static List<Pawn> GetControlledWalkingCorpses(Pawn mechanitor)
		{
			List<Pawn> list = new();
			List<Pawn> mechs = mechanitor.mechanitor.ControlledPawns;
			foreach (Pawn item in mechs)
			{
				if (PawnIsBoneGolem(item) && !item.health.Dead)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static bool PawnIsBoneGolem(Pawn pawn)
		{
			if (pawn.RaceProps.IsMechanoid)
			{
				return pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_SelfPopulationRegulation_BoneGolems);
			}
			return false;
		}


	}
}
