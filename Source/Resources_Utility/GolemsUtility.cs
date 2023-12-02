using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class GolemsUtility
	{

		public static int TotalGolemBandwidth(Pawn mechanitor)
		{
			return (int)mechanitor.GetStatValue(WVC_GenesDefOf.WVC_OverseerMaxGolems);
		}

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
			int maxGolems = TotalGolemBandwidth(mechanitor);
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
			List <Pawn> golems = GetControlledGolems(mechanitor);
			if (golems.NullOrEmpty())
			{
				return result;
			}
			foreach (Pawn golem in golems)
			{
				int golemBand = (int)golem.GetStatValue(WVC_GenesDefOf.WVC_GolemBandwidthCost);
				if (golemBand > 0)
				{
					result += golemBand;
				}
			}
			// Log.Error("GetConsumedBandwidth: " + result);
			return result;
		}

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
