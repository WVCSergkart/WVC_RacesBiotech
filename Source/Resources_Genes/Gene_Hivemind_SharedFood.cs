using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Hivemind_SharedFood : Gene_Hivemind
	{

		public override void UpdGeneSync()
		{
			try
			{
				List<Pawn> hivemind = Hivemind;
				float totalNutriotion = 0;
				float factor = Mathf.Clamp(PsyFactor, 0.1f, 1f);
				foreach (Pawn pawn in hivemind)
				{
					if (!pawn.TryGetNeedFood(out Need_Food need_Food))
					{
						continue;
					}
					totalNutriotion += need_Food.CurLevel * factor;
				}
				float averageNutrition = totalNutriotion / hivemind.Count;
				foreach (Pawn pawn in hivemind)
				{
					if (!pawn.TryGetNeedFood(out Need_Food need_Food))
					{
						continue;
					}
					need_Food.CurLevel = averageNutrition;
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed share food for hivemind. Reason: " + arg.Message);
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
