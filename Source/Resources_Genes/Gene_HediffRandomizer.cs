using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_HediffRandomizer : Gene
	{
		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
		public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		public override void PostAdd()
		{
			base.PostAdd();
			HediffGiver(Bodyparts, HediffDefName, pawn, Active);
		}

		public override void Tick()
		{
			base.Tick();
			// if (!pawn.IsHashIntervalTick(1500))
			if (!pawn.IsHashIntervalTick(300000))
			{
				return;
			}
			HediffGiver(Bodyparts, HediffDefName, pawn, Active);
		}

		public static void HediffGiver(List<BodyPartDef> bodyparts, HediffDef hediffDefName, Pawn pawn, bool active)
		{
			if (active && Rand.Chance(0.2f))
			{
				int num = 0;
				foreach (BodyPartDef bodypart in bodyparts)
				{
					if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
					{
						pawn.health.AddHediff(hediffDefName, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
						num++;
					}
				}
			}
		}
	}

}
