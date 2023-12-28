using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_InstantKill : HediffCompProperties
	{

		public HediffCompProperties_InstantKill()
		{
			compClass = typeof(HediffComp_InstantKill);
		}
	}

	public class HediffComp_InstantKill : HediffComp
	{

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.Pawn.Kill(null, parent);
		}

	}

	public class HediffCompProperties_GolemLimit : HediffCompProperties
	{

		public int checkInterval = 60000;

		// public StatDef statDef;

		// public HediffCompProperties_GolemPopulationRegulation()
		// {
			// compClass = typeof(HediffComp_GolemPopulationRegulation);
		// }
	}

	public class HediffComp_GolemLimit_Stone : HediffComp
	{

		public HediffCompProperties_GolemLimit Props => (HediffCompProperties_GolemLimit)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(Props.checkInterval))
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

	public class HediffComp_GolemLimit_Bone : HediffComp
	{

		public HediffCompProperties_GolemLimit Props => (HediffCompProperties_GolemLimit)props;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(Props.checkInterval))
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
			Pawn overseer = golem.GetOverseer();
			if (!WalkingUtility.LimitExceedCheck(overseer))
			{
				base.Pawn.Kill(null, parent);
			}
		}

	}

}
