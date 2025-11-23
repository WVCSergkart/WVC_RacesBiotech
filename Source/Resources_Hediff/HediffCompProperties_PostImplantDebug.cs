using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_PostImplantDebug : HediffCompProperties
	{

		public HediffCompProperties_PostImplantDebug()
		{
			compClass = typeof(HediffComp_PostImplantDebug);
		}

	}

	public class HediffComp_PostImplantDebug : HediffComp
	{

		public override void CompPostPostRemoved()
		{
			ReimplanterUtility.PostImplantDebug(Pawn);
		}

	}

	public class HediffCompProperties_XenogermReplicating : HediffCompProperties
	{

		public HediffCompProperties_XenogermReplicating()
		{
			compClass = typeof(HediffComp_XenogermReplicating);
		}

	}

	public class HediffComp_XenogermReplicating : HediffComp
	{

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			if (Pawn.IsHashIntervalTick(55555, delta))
			{
				if (Rand.Chance(0.1f) && !Pawn.health.hediffSet.AnyHediffMakesSickThought)
				{
					ReimplanterUtility.XenogermReplicating_WithCustomDuration(Pawn, new IntRange(-60000 * 30, -60000 * 15), parent);
				}
			}
		}

	}

}
