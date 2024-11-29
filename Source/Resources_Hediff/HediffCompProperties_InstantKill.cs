using System;
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

	// [Obsolete]
	// public class HediffCompProperties_GolemLimit : HediffCompProperties
	// {

		// public IntRange checkInterval = new(45000, 85000);

	// }

	// [Obsolete]
	// public class HediffComp_GolemLimit_Stone : HediffComp
	// {

		// public int checkInterval = 0;

		// public HediffCompProperties_GolemLimit Props => (HediffCompProperties_GolemLimit)props;

		// public override void CompPostMake()
		// {
			// checkInterval = Props.checkInterval.RandomInRange;
		// }

		// public override void CompPostTick(ref float severityAdjustment)
		// {
			// base.CompPostTick(ref severityAdjustment);
			// if (!Pawn.IsHashIntervalTick(checkInterval))
			// {
				// return;
			// }
			// Pawn golem = parent.pawn;
			// if (golem.GetOverseer() == null)
			// {
				// base.Pawn.Kill(null, parent);
				// return;
			// }
			// if (golem.Map == null)
			// {
				// return;
			// }
			// Pawn overseer = golem.GetOverseer();
			// if (!GolemsUtility.LimitExceedCheck(overseer) || !MechanoidsUtility.MechanitorIsGolemist(overseer))
			// {
				// base.Pawn.Kill(null, parent);
			// }
			// checkInterval = Props.checkInterval.RandomInRange;
		// }

		// public override void CompExposeData()
		// {
			// base.CompExposeData();
			// Scribe_Values.Look(ref checkInterval, "checkInterval", 0);
		// }

	// }

	// [Obsolete]
	// public class HediffComp_GolemLimit_Bone : HediffComp_GolemLimit_Stone
	// {

		// public override void CompPostTick(ref float severityAdjustment)
		// {
			// base.CompPostTick(ref severityAdjustment);
			// if (!Pawn.IsHashIntervalTick(checkInterval))
			// {
				// return;
			// }
			// Pawn golem = parent.pawn;
			// if (golem.GetOverseer() == null)
			// {
				// base.Pawn.Kill(null, parent);
				// return;
			// }
			// if (golem.Map == null)
			// {
				// return;
			// }
			// Pawn overseer = golem.GetOverseer();
			// if (!WalkingUtility.LimitExceedCheck(overseer) || !MechanoidsUtility.MechanitorIsLich(overseer))
			// {
				// base.Pawn.Kill(null, parent);
			// }
		// }

	// }

}
