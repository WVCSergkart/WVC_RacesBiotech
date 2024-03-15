using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

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
			weight = TotalSporesBandwidth(lich) - MechanoidsUtility.GetConsumedGolembond(lich) - golem.race.statBases.GetStatValueFromList(WVC_GenesDefOf.WVC_GolemBondCost, 100f);
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
				float weight = TotalSporesBandwidth(lich) - MechanoidsUtility.GetConsumedGolembond(lich) - golem.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost);
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
			float weight = MechanoidsUtility.TotalGolembond(lich) - MechanoidsUtility.GetConsumedGolembond(lich);
			if (weight <= 0f)
			{
				weight = 0.001f;
			}
			return weight;
		}

		public static List<Pawn> GetAllLichs(Map map)
		{
			List<Pawn> list = new();
			List<Pawn> colonists = map?.mapPawns?.FreeColonists;
			if (!colonists.NullOrEmpty())
			{
				foreach (Pawn colonist in colonists)
				{
					Gene_Sporelink sporelink = colonist?.genes?.GetFirstGeneOfType<Gene_Sporelink>();
					if (sporelink == null)
					{
						continue;
					}
					if (!sporelink.summonMechanoids)
					{
						continue;
					}
					list.Add(colonist);
				}
			}
			return list;
		}

		// [Obsolete]
		// public static bool MechanitorIsLich(Pawn mechanitor)
		// {
			// if (!MechanitorUtility.IsMechanitor(mechanitor))
			// {
				// return false;
			// }
			// Gene_ResurgentMechlink gene_ResurgentMechlink = mechanitor.genes?.GetFirstGeneOfType<Gene_ResurgentMechlink>();
			// if (gene_ResurgentMechlink != null && XaG_GeneUtility.HasActiveGene(gene_ResurgentMechlink.def, mechanitor))
			// {
				// if (TotalSporesBandwidth(mechanitor) > 0)
				// {
					// return true;
				// }
			// }
			// return false;
		// }

		public static float TotalSporesBandwidth(Pawn mechanitor)
		{
			return mechanitor.GetStatValue(WVC_GenesDefOf.WVC_GolemBond);
		}

		// [Obsolete]
		// public static bool MechanitorHasAnyWalkingCorpses(Pawn mechanitor)
		// {
			// List<Pawn> list = GetControlledWalkingCorpses(mechanitor);
			// if (list.NullOrEmpty())
			// {
				// return false;
			// }
			// return true;
		// }

		public static bool LimitExceedCheck(Pawn mechanitor)
		{
			float maxGolems = TotalSporesBandwidth(mechanitor);
			// if (maxGolems <= 0f)
			// {
				// return false;
			// }
			float bandwidthCost = MechanoidsUtility.GetConsumedGolembond(mechanitor);
			if (maxGolems >= bandwidthCost)
			{
				return true;
			}
			return false;
		}

		// [Obsolete]
		// public static float GetConsumedBandwidth(Pawn mechanitor)
		// {
			// float result = 0;
			// List <Pawn> golems = GetControlledWalkingCorpses(mechanitor);
			// if (golems.NullOrEmpty())
			// {
				// return result;
			// }
			// foreach (Pawn golem in golems)
			// {
				// float golemBand = golem.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost);
				// if (golemBand > 0)
				// {
					// result += golemBand;
				// }
			// }
			// return result;
		// }

		// [Obsolete]
		// public static List<Pawn> GetControlledWalkingCorpses(Pawn mechanitor)
		// {
			// List<Pawn> list = new();
			// List<Pawn> mechs = mechanitor.mechanitor.ControlledPawns;
			// foreach (Pawn item in mechs)
			// {
				// if (PawnIsBoneGolem(item) && !item.health.Dead)
				// {
					// list.Add(item);
				// }
			// }
			// return list;
		// }

		// [Obsolete]
		// public static bool PawnIsBoneGolem(Pawn pawn)
		// {
			// if (pawn.RaceProps.IsMechanoid)
			// {
				// return pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_SelfPopulationRegulation_BoneGolems);
			// }
			// return false;
		// }


	}
}
