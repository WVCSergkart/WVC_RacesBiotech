using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_PermanentHediff : Gene
	{

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
		public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		public override void PostAdd()
		{
			base.PostAdd();
			// ResetChance();
			// Log.Error("Ролим шанс. Шанс = " + chanceRange);
			// if (Active && chanceRange == 3)
			// {
			// }
			if (!pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				// Gene_RandomHediffFromTime.HediffGiver(def.GetModExtension<GeneExtension_Giver>().bodyparts, HediffDefName, pawn);
				// int num = 0;
				// foreach (BodyPartDef bodypart in def.GetModExtension<GeneExtension_Giver>().bodyparts)
				// {
					// if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
					// {
						// pawn.health.AddHediff(HediffDefName, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
						// num++;
					// }
				// }
				int num = 0;
				foreach (BodyPartDef bodypart in Bodyparts)
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

}
