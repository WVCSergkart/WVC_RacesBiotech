using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class GolemsUtility
	{

		public static bool CanSpawnMoreGolems(Pawn lich, Pawn golem)
		{
			if (MechanoidsUtility.CanSpawnMoreMechanoids(lich, golem))
			{
				float weight = TotalGolemBandwidth(lich) - MechanoidsUtility.GetConsumedGolembond(lich) - golem.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost);
				if (weight < 0f)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		// =====================================

		public static float TotalGolemBandwidth(Pawn mechanitor)
		{
			return mechanitor.GetStatValue(WVC_GenesDefOf.WVC_GolemBond);
		}

		[Obsolete]
		public static bool MechanitorHasAnyGolems(Pawn mechanitor)
		{
			List<Pawn> list = GetControlledGolems(mechanitor);
			if (list.NullOrEmpty())
			{
				return false;
			}
			return true;
		}

		public static bool LimitExceedCheck(Pawn mechanitor)
		{
			float maxGolems = TotalGolemBandwidth(mechanitor);
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

		[Obsolete]
		public static float GetConsumedBandwidth(Pawn mechanitor)
		{
			float result = 0;
			List <Pawn> golems = GetControlledGolems(mechanitor);
			if (golems.NullOrEmpty())
			{
				return result;
			}
			foreach (Pawn golem in golems)
			{
				float golemBand = golem.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost);
				if (golemBand > 0)
				{
					result += golemBand;
				}
			}
			// Log.Error("GetConsumedBandwidth: " + result);
			return result;
		}

		[Obsolete]
		public static List<Pawn> GetControlledGolems(Pawn mechanitor)
		{
			List<Pawn> list = new();
			List<Pawn> mechs = mechanitor.mechanitor.ControlledPawns;
			foreach (Pawn item in mechs)
			{
				if (PawnIsGolem(item) && !item.health.Dead)
				{
					list.Add(item);
				}
			}
			return list;
		}

		// [Obsolete]
		public static bool PawnIsGolem(Pawn pawn)
		{
			if (pawn.RaceProps.IsMechanoid)
			{
				return pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_SelfPopulationRegulation_Golems);
			}
			return false;
		}


	}
}
