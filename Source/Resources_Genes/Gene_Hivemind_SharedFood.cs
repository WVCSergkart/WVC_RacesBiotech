using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Hivemind_SharedFood : Gene_Hivemind_Drone
	{

		public static int delayBeforeSync;
		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				TryUpdFood();
			}
		}

		public void TryUpdFood()
		{
			int ticksGame = Find.TickManager.TicksGame;
			if (delayBeforeSync > ticksGame)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (!HivemindUtility.InHivemind(pawn))
			{
				return;
			}
			string phase = "start";
			try
			{
				List<Pawn> hivemind = Hivemind;
				float totalNutriotion = 0;
				phase = "get total hivemind food";
				foreach (Pawn pawn in hivemind)
				{
					if (!pawn.TryGetNeedFood(out Need_Food need_Food))
					{
						continue;
					}
					totalNutriotion += need_Food.CurLevel;
					need_Food.CurLevel = 0;
				}
				phase = "factor for total food";
				totalNutriotion *= PsyFactor;
				phase = "try share food between pawns";
				foreach (Pawn pawn in hivemind)
				{
					if (totalNutriotion <= 0)
					{
						break;
					}
					if (!pawn.TryGetNeedFood(out Need_Food need_Food))
					{
						continue;
					}
					float foodValue = Mathf.Clamp(need_Food.NutritionWanted, 0f, totalNutriotion);
					need_Food.CurLevel += foodValue;
					totalNutriotion -= foodValue;
					if (totalNutriotion < 0)
					{
						break;
					}
				}
				phase = "set delay";
				delayBeforeSync = ticksGame + Mathf.Clamp((int)(320 * hivemind.Count / PsyFactor), 2500, HivemindUtility.TickRefresh);
			}
			catch (Exception arg)
			{
				Log.Error($"Failed share food for hivemind. On phase: {phase}. Reason: " + arg.Message);
				delayBeforeSync = 60000;
			}
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active || !HivemindUtility.SuitableForHivemind(pawn))
			{
				return;
			}
			float nutrition = (thing.def.ingestible.CachedNutrition * numTaken) * 0.1f;
			foreach (Pawn hiver in Hivemind)
			{
				if (hiver == pawn)
				{
					continue;
				}
				if (hiver.TryGetNeedFood(out Need_Food need_Food))
				{
					need_Food.CurLevel += nutrition;
				}
			}
		}

	}

}
