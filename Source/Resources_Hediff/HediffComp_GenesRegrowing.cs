using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class HediffCompProperties_RemoveHediffFromList : HediffCompProperties
	{

		public List<HediffDef> hediffs;

		public int refreshTicks = 6000;

		public HediffCompProperties_RemoveHediffFromList()
		{
			compClass = typeof(HediffComp_RemoveHediffFromList);
		}
	}

	[Obsolete]
	public class HediffComp_RemoveHediffFromList : HediffComp
	{

		public HediffCompProperties_RemoveHediffFromList Props => (HediffCompProperties_RemoveHediffFromList)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			if (!Pawn.IsHashIntervalTick(Props.refreshTicks))
			{
				return;
			}
			// XaG_GeneUtility.XenogermRestoration(pawn);
			List<Hediff> hediffs = Pawn?.health?.hediffSet?.hediffs;
			if (hediffs.NullOrEmpty() || Props.hediffs.NullOrEmpty())
			{
				return;
			}
			foreach (Hediff item in hediffs.ToList())
			{
				if (!Props.hediffs.Contains(item.def))
				{
					continue;
				}
				Pawn.health.RemoveHediff(item);
			}
		}

	}

}
