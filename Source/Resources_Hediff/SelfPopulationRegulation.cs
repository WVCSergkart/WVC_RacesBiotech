using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_GolemPopulationRegulation : HediffCompProperties
	{

		// public StatDef statDef;

		public HediffCompProperties_GolemPopulationRegulation()
		{
			compClass = typeof(HediffComp_GolemPopulationRegulation);
		}
	}

	public class HediffComp_GolemPopulationRegulation : HediffComp
	{

		// public HediffCompProperties_GolemPopulationRegulation Props => (HediffCompProperties_GolemPopulationRegulation)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(60000))
			{
				return;
			}
			Pawn golem = parent.pawn;
			if (golem.GetOverseer() == null)
			{
				base.Pawn.Kill(null, parent);
				return;
			}
			if (golem.Map == null)
			{
				return;
			}
			// if (!Pawn.IsHashIntervalTick(120000))
			// {
				// return;
			// }
			Pawn overseer = golem.GetOverseer();
			// int connectedThingsCount = 0;
			// List<Pawn> connectedThingThing = overseer.mechanitor.ControlledPawns;
			// foreach (Pawn item in connectedThingThing)
			// {
				// if (!item.health.Downed && !item.health.Dead && item.kindDef.defName.Contains(golem.kindDef.defName))
				// {
					// connectedThingsCount++;
				// }
			// }
			if (!GolemsUtility.LimitExceedCheck(overseer))
			{
				base.Pawn.Kill(null, parent);
			}
		}

	}

}
