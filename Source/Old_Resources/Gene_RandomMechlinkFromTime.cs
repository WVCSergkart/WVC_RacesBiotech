using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC
{

	public class Gene_RandomHediffFromTime : Gene
	{
		// public BodyPartDef Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		private IntRange spawnCountRange = new(1, 5);

		private int chanceRange;

		// private const int ClotCheckInterval = 750;

		// private static readonly FloatRange TendingQualityRange = new FloatRange(0.5f, 1.0f);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetChance();
			// Log.Error("Ролим шанс. Шанс = " + chanceRange);
			if (Active && chanceRange == 3)
			{
				int num = 0;
				foreach (BodyPartDef bodypart in def.GetModExtension<GeneExtension_Giver>().bodyparts)
				{
					if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
					{
						pawn.health.AddHediff(HediffDefName, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
						num++;
					}
				}
			}
		}

		public override void Tick()
		{
			base.Tick();
			// if (!pawn.IsHashIntervalTick(1500))
			if (!pawn.IsHashIntervalTick(300000))
			{
				return;
			}
			ResetChance();
			// Log.Error("Ролим шанс. Шанс = " + chanceRange);
			if (Active && chanceRange == 3)
			{
				if (pawn.Map != null)
				{
					int num = 0;
					foreach (BodyPartDef bodypart in def.GetModExtension<GeneExtension_Giver>().bodyparts)
					{
						if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
						{
							pawn.health.AddHediff(HediffDefName, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
							num++;
						}
					}
				}
			}
		}

		private void ResetChance()
		{
			chanceRange = spawnCountRange.RandomInRange;
		}
	}

}
